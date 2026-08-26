from __future__ import annotations

import argparse
import os
import re
import sqlite3
import sys
from contextlib import closing
from dataclasses import dataclass
from pathlib import Path
from typing import Mapping, Sequence, TextIO


SOURCE_ENVIRONMENT_VARIABLE = "AIRCRAFT_TRACKER_DB"
TARGET_ENVIRONMENT_VARIABLE = "FLIGHT_RECORDER_DB"
AIRCRAFT_TABLE = "AIRCRAFT"
SOURCE_COLUMNS = frozenset({"address", "registration"})
TARGET_COLUMNS = frozenset({"address", "registration"})
ICAO_ADDRESS_PATTERN = re.compile(r"^[0-9A-F]{6}$")


class ImportValidationError(Exception):
    """Raised when the import configuration or database schema is invalid."""


@dataclass(frozen=True)
class ImportSummary:
    """Counts describing the outcome of an import."""

    missing_targets: int
    updated: int
    unmatched: int
    invalid_source_address: int
    ambiguous: int
    ambiguous_registrations: tuple[str, ...]
    dry_run: bool


def normalize_registration(value: object) -> str:
    """Normalize an aircraft registration for matching.

    :param value: Registration value read from a database.
    :return: The trimmed, uppercase registration, or an empty string.
    """
    return "" if value is None else str(value).strip().upper()


def normalize_icao_address(value: object) -> str | None:
    """Validate and normalize a 24-bit ICAO address.

    :param value: Address value read from the source database.
    :return: An uppercase six-character address, or None when invalid.
    """
    address = "" if value is None else str(value).strip().upper()
    return address if ICAO_ADDRESS_PATTERN.fullmatch(address) else None


def database_uri(path: Path, mode: str, immutable: bool = False) -> str:
    """Build a SQLite URI for an existing database file.

    :param path: Resolved path to the SQLite database.
    :param mode: SQLite access mode, such as ro or rw.
    :param immutable: Whether SQLite should avoid journal and locking access.
    :return: A SQLite file URI containing the requested access mode.
    """
    immutable_option = "&immutable=1" if immutable else ""
    return f"{path.as_uri()}?mode={mode}{immutable_option}"


def open_database(
    path: Path,
    read_only: bool,
    immutable: bool = False,
) -> sqlite3.Connection:
    """Open an existing SQLite database without allowing implicit creation.

    :param path: Resolved path to the database.
    :param read_only: Whether the connection must prohibit writes.
    :param immutable: Whether the read-only file is a stable, unchanging source.
    :return: An open SQLite connection.
    """
    if immutable and not read_only:
        raise ValueError("Immutable mode requires a read-only connection")

    mode = "ro" if read_only else "rw"
    connection = sqlite3.connect(
        database_uri(path, mode, immutable=immutable),
        uri=True,
    )
    connection.execute("PRAGMA busy_timeout = 5000")
    return connection


def resolve_database_path(
    command_line_value: str | None,
    environment_name: str,
    environment: Mapping[str, str],
    description: str,
) -> Path:
    """Resolve and validate a database path from CLI or environment.

    :param command_line_value: Path supplied on the command line, if any.
    :param environment_name: Name of the fallback environment variable.
    :param environment: Environment mapping used to find the fallback.
    :param description: Human-readable database description for errors.
    :return: The absolute, resolved database path.
    """
    raw_path = command_line_value or environment.get(environment_name)
    if not raw_path:
        raise ImportValidationError(
            f"{description} database path is required; use its command-line "
            f"option or set {environment_name}"
        )

    path = Path(raw_path).expanduser()
    if not path.exists():
        raise ImportValidationError(f"{description} database does not exist: {path}")
    if not path.is_file():
        raise ImportValidationError(f"{description} database is not a file: {path}")
    if not os.access(path, os.R_OK):
        raise ImportValidationError(f"{description} database is not readable: {path}")
    return path.resolve()


def validate_aircraft_schema(
    connection: sqlite3.Connection,
    expected_columns: frozenset[str],
    description: str,
) -> None:
    """Validate the AIRCRAFT table and its required columns.

    :param connection: Open database connection to inspect.
    :param expected_columns: Lowercase column names required by the importer.
    :param description: Human-readable database description for errors.
    :return: None.
    """
    table = connection.execute(
        "SELECT name FROM sqlite_master WHERE type = ? AND UPPER(name) = ?",
        ("table", AIRCRAFT_TABLE),
    ).fetchone()
    if table is None:
        raise ImportValidationError(
            f"{description} database does not contain an {AIRCRAFT_TABLE} table"
        )

    columns = {
        str(row[1]).lower()
        for row in connection.execute(f'PRAGMA table_info("{AIRCRAFT_TABLE}")')
    }
    missing_columns = expected_columns - columns
    if missing_columns:
        names = ", ".join(sorted(missing_columns))
        raise ImportValidationError(
            f"{description} {AIRCRAFT_TABLE} table is missing column(s): {names}"
        )


def load_source_addresses(
    connection: sqlite3.Connection,
) -> dict[str, set[str]]:
    """Load valid source addresses grouped by normalized registration.

    Registrations with rows but no valid address remain in the mapping with an
    empty set, distinguishing them from registrations absent from the source.

    :param connection: Read-only BaseStation Reader database connection.
    :return: Mapping of normalized registrations to distinct valid addresses.
    """
    addresses_by_registration: dict[str, set[str]] = {}
    rows = connection.execute(
        'SELECT "Registration", "Address" FROM "AIRCRAFT"'
    )
    for registration_value, address_value in rows:
        registration = normalize_registration(registration_value)
        if not registration:
            continue

        addresses = addresses_by_registration.setdefault(registration, set())
        address = normalize_icao_address(address_value)
        if address is not None:
            addresses.add(address)

    return addresses_by_registration


def build_update_plan(
    connection: sqlite3.Connection,
    source_addresses: Mapping[str, set[str]],
) -> tuple[dict[str, str], int, int, int, int, int, tuple[str, ...]]:
    """Classify missing target addresses and build an update plan.

    :param connection: Flight Recorder database connection.
    :param source_addresses: Valid source addresses grouped by registration.
    :return: Plan, eligible count, missing count, unmatched count, invalid
        count, ambiguous count, and ambiguous registrations.
    """
    plan: dict[str, str] = {}
    eligible = 0
    missing_targets = 0
    unmatched = 0
    invalid = 0
    ambiguous = 0
    ambiguous_registrations: set[str] = set()

    rows = connection.execute(
        """
        SELECT registration
        FROM AIRCRAFT
        WHERE COALESCE(TRIM(address), '') = ''
        ORDER BY rowid
        """
    )
    for (registration_value,) in rows:
        missing_targets += 1
        registration = normalize_registration(registration_value)
        if registration not in source_addresses:
            unmatched += 1
            continue

        addresses = source_addresses[registration]
        if not addresses:
            invalid += 1
        elif len(addresses) > 1:
            ambiguous += 1
            ambiguous_registrations.add(registration)
        else:
            eligible += 1
            plan[registration] = next(iter(addresses))

    return (
        plan,
        eligible,
        missing_targets,
        unmatched,
        invalid,
        ambiguous,
        tuple(sorted(ambiguous_registrations)),
    )


def apply_update_plan(
    connection: sqlite3.Connection,
    plan: Mapping[str, str],
) -> int:
    """Apply planned target updates in a single transaction.

    :param connection: Writable Flight Recorder database connection.
    :param plan: Normalized registrations mapped to ICAO addresses.
    :return: Number of target rows updated.
    """
    updated = 0
    try:
        # IMMEDIATE obtains the write reservation before any aircraft changes.
        connection.execute("BEGIN IMMEDIATE")
        for registration, address in plan.items():
            cursor = connection.execute(
                """
                UPDATE AIRCRAFT
                SET address = ?
                WHERE UPPER(TRIM(registration)) = ?
                  AND COALESCE(TRIM(address), '') = ''
                """,
                (address, registration),
            )
            updated += cursor.rowcount
        connection.commit()
    except Exception:
        connection.rollback()
        raise
    return updated


def import_addresses(
    source_path: Path,
    target_path: Path,
    dry_run: bool = False,
) -> ImportSummary:
    """Import eligible ICAO addresses into Flight Recorder.

    :param source_path: Resolved BaseStation Reader database path.
    :param target_path: Resolved Flight Recorder database path.
    :param dry_run: Whether to report changes without applying them.
    :return: Counts describing the completed import.
    """
    if source_path.resolve() == target_path.resolve():
        raise ImportValidationError(
            "Source and target database paths must identify different files"
        )

    try:
        # BaseStation databases may be stored where SQLite cannot create lock or
        # journal sidecars. Immutable mode guarantees that this source remains
        # read-only and prevents SQLite from attempting those filesystem writes.
        with closing(
            open_database(source_path, read_only=True, immutable=True)
        ) as source_connection:
            validate_aircraft_schema(
                source_connection, SOURCE_COLUMNS, "Source BaseStation Reader"
            )
            source_addresses = load_source_addresses(source_connection)

        # A dry run opens the target read-only, guaranteeing no mutation.
        with closing(
            open_database(target_path, read_only=dry_run)
        ) as target_connection:
            validate_aircraft_schema(
                target_connection, TARGET_COLUMNS, "Target Flight Recorder"
            )
            (
                plan,
                eligible,
                missing_targets,
                unmatched,
                invalid,
                ambiguous,
                ambiguous_registrations,
            ) = build_update_plan(target_connection, source_addresses)
            updated = eligible if dry_run else apply_update_plan(target_connection, plan)
    except ImportValidationError:
        raise
    except sqlite3.Error as error:
        raise ImportValidationError(f"SQLite database error: {error}") from error

    return ImportSummary(
        missing_targets=missing_targets,
        updated=updated,
        unmatched=unmatched,
        invalid_source_address=invalid,
        ambiguous=ambiguous,
        ambiguous_registrations=ambiguous_registrations,
        dry_run=dry_run,
    )


def create_argument_parser() -> argparse.ArgumentParser:
    """Create the command-line argument parser.

    :return: Configured argument parser.
    """
    parser = argparse.ArgumentParser()
    parser.add_argument("-s", "--source-db", help="BaseStation Reader database")
    parser.add_argument("-t", "--target-db", help="Flight Recorder database")
    parser.add_argument("-d", "--dry-run",action="store_true",
                        help="report proposed updates without changing the target database")
    return parser


def print_summary(
    summary: ImportSummary,
    source_path: Path,
    target_path: Path,
    output: TextIO,
    error_output: TextIO,
) -> None:
    """Print an operator-friendly import summary.

    :param summary: Completed import counts.
    :param source_path: Resolved source database path.
    :param target_path: Resolved target database path.
    :param output: Standard output destination.
    :param error_output: Standard error destination.
    :return: None.
    """
    action = "Would update" if summary.dry_run else "Updated"
    print(f"Source database: {source_path}", file=output)
    print(f"Target database: {target_path}", file=output)
    print(f"Dry run: {'yes' if summary.dry_run else 'no'}", file=output)
    print(f"Target records with missing addresses: {summary.missing_targets}", file=output)
    print(f"{action}: {summary.updated}", file=output)
    print(f"Unmatched registrations: {summary.unmatched}", file=output)
    print(f"Invalid or missing source addresses: {summary.invalid_source_address}", file=output)
    print(f"Ambiguous source registrations: {summary.ambiguous}", file=output)
    if summary.ambiguous_registrations:
        registrations = ", ".join(summary.ambiguous_registrations)
        print(f"Warning: ambiguous registrations skipped: {registrations}", file=error_output)
    print("Import completed successfully", file=output)


def main(
    arguments: Sequence[str] | None = None,
    environment: Mapping[str, str] | None = None,
    output: TextIO = sys.stdout,
    error_output: TextIO = sys.stderr,
) -> int:
    """Run the aircraft ICAO address importer.

    :param arguments: Command-line arguments excluding the executable name.
    :param environment: Environment-variable mapping; defaults to os.environ.
    :param output: Standard output destination.
    :param error_output: Standard error destination.
    :return: Process exit status: zero on success and non-zero on failure.
    """
    parser = create_argument_parser()
    parsed = parser.parse_args(arguments)
    current_environment = os.environ if environment is None else environment

    try:
        source_path = resolve_database_path(
            parsed.source_db,
            SOURCE_ENVIRONMENT_VARIABLE,
            current_environment,
            "Source BaseStation Reader",
        )
        target_path = resolve_database_path(
            parsed.target_db,
            TARGET_ENVIRONMENT_VARIABLE,
            current_environment,
            "Target Flight Recorder",
        )
        summary = import_addresses(source_path, target_path, parsed.dry_run)
        print_summary(summary, source_path, target_path, output, error_output)
        return 0
    except (ImportValidationError, OSError) as error:
        print(f"Import failed: {error}", file=error_output)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
