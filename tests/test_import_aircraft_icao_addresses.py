"""Tests for the aircraft ICAO address importer."""

from __future__ import annotations

import io
import sqlite3
import tempfile
import unittest
from contextlib import closing
from pathlib import Path

from scripts.import_aircraft_icao_addresses import (
    ImportValidationError,
    database_uri,
    import_addresses,
    main,
    normalize_icao_address,
    resolve_database_path,
)


class AircraftIcaoAddressImportTests(unittest.TestCase):
    """Exercise imports using isolated temporary SQLite databases."""

    def setUp(self) -> None:
        """Create temporary source and target database paths.

        :return: None.
        """
        self.temporary_directory = tempfile.TemporaryDirectory()
        directory = Path(self.temporary_directory.name)
        self.source_path = directory / "source.db"
        self.target_path = directory / "target.db"
        self._create_source([])
        self._create_target([])

    def tearDown(self) -> None:
        """Remove temporary database files.

        :return: None.
        """
        self.temporary_directory.cleanup()

    def _create_source(self, rows: list[tuple[object, object]]) -> None:
        """Create a BaseStation-style source database.

        :param rows: Registration and address values to insert.
        :return: None.
        """
        self.source_path.unlink(missing_ok=True)
        with closing(sqlite3.connect(self.source_path)) as connection, connection:
            connection.execute(
                "CREATE TABLE AIRCRAFT (Registration TEXT, Address TEXT)"
            )
            connection.executemany(
                "INSERT INTO AIRCRAFT (Registration, Address) VALUES (?, ?)", rows
            )

    def _create_target(self, rows: list[tuple[object, object]]) -> None:
        """Create a Flight Recorder-style target database.

        :param rows: Registration and address values to insert.
        :return: None.
        """
        self.target_path.unlink(missing_ok=True)
        with closing(sqlite3.connect(self.target_path)) as connection, connection:
            connection.execute(
                "CREATE TABLE AIRCRAFT "
                "(id INTEGER PRIMARY KEY, registration TEXT, address TEXT)"
            )
            connection.executemany(
                "INSERT INTO AIRCRAFT (registration, address) VALUES (?, ?)", rows
            )

    def _target_rows(self) -> list[tuple[object, object]]:
        """Read target registration and address values.

        :return: Target rows ordered by their generated identifier.
        """
        with closing(sqlite3.connect(self.target_path)) as connection:
            return connection.execute(
                "SELECT registration, address FROM AIRCRAFT ORDER BY id"
            ).fetchall()

    def test_imports_normalized_address_using_normalized_registration(self) -> None:
        """A matching missing address is imported and normalized.

        :return: None.
        """
        self._create_source([(" g-abcd ", " a1b2c3 ")])
        self._create_target([("G-ABCD", None)])

        summary = import_addresses(self.source_path, self.target_path)

        self.assertEqual([("G-ABCD", "A1B2C3")], self._target_rows())
        self.assertEqual(1, summary.updated)

    def test_preserves_populated_target_and_does_not_insert_source_only_row(self) -> None:
        """Populated targets and source-only aircraft remain untouched.

        :return: None.
        """
        self._create_source([("G-KEPT", "AAAAAA"), ("G-NEW", "BBBBBB")])
        self._create_target([("G-KEPT", "123456")])

        summary = import_addresses(self.source_path, self.target_path)

        self.assertEqual([("G-KEPT", "123456")], self._target_rows())
        self.assertEqual(0, summary.updated)

    def test_null_empty_and_whitespace_targets_are_eligible(self) -> None:
        """Every representation of a missing target address is updated.

        :return: None.
        """
        self._create_source(
            [
                ("G-NULL", "AAAAAA"),
                ("G-EMPTY", "BBBBBB"),
                ("G-SPACE", "CCCCCC"),
            ]
        )
        self._create_target(
            [("G-NULL", None), ("G-EMPTY", ""), ("G-SPACE", "   ")]
        )

        summary = import_addresses(self.source_path, self.target_path)

        self.assertEqual(3, summary.updated)
        self.assertEqual(
            [
                ("G-NULL", "AAAAAA"),
                ("G-EMPTY", "BBBBBB"),
                ("G-SPACE", "CCCCCC"),
            ],
            self._target_rows(),
        )

    def test_classifies_unmatched_invalid_and_ambiguous_targets(self) -> None:
        """Data-quality problems are counted and skipped.

        :return: None.
        """
        self._create_source(
            [
                ("G-INVALID", "XYZ"),
                ("G-AMBIG", "AAAAAA"),
                ("g-ambig", "BBBBBB"),
            ]
        )
        self._create_target(
            [("G-MISSING", None), ("G-INVALID", None), ("G-AMBIG", None)]
        )

        summary = import_addresses(self.source_path, self.target_path)

        self.assertEqual(1, summary.unmatched)
        self.assertEqual(1, summary.invalid_source_address)
        self.assertEqual(1, summary.ambiguous)
        self.assertEqual(("G-AMBIG",), summary.ambiguous_registrations)
        self.assertEqual(0, summary.updated)

    def test_duplicate_identical_source_rows_produce_one_address(self) -> None:
        """Equivalent source duplicates do not make a match ambiguous.

        :return: None.
        """
        self._create_source([("G-DUPE", "ABCDEF"), ("g-dupe", "abcdef")])
        self._create_target([("G-DUPE", None)])

        summary = import_addresses(self.source_path, self.target_path)

        self.assertEqual(1, summary.updated)
        self.assertEqual([("G-DUPE", "ABCDEF")], self._target_rows())

    def test_dry_run_reports_updates_without_modifying_target(self) -> None:
        """Dry-run mode leaves the target database unchanged.

        :return: None.
        """
        self._create_source([("G-DRY", "ABCDEF")])
        self._create_target([("G-DRY", None)])

        summary = import_addresses(self.source_path, self.target_path, dry_run=True)

        self.assertTrue(summary.dry_run)
        self.assertEqual(1, summary.updated)
        self.assertEqual([("G-DRY", None)], self._target_rows())

    def test_second_run_is_idempotent(self) -> None:
        """A completed import has no eligible update on its second run.

        :return: None.
        """
        self._create_source([("G-ONCE", "ABCDEF")])
        self._create_target([("G-ONCE", None)])

        first = import_addresses(self.source_path, self.target_path)
        second = import_addresses(self.source_path, self.target_path)

        self.assertEqual(1, first.updated)
        self.assertEqual(0, second.updated)
        self.assertEqual(0, second.missing_targets)

    def test_failed_update_rolls_back_prior_updates(self) -> None:
        """A database error during processing rolls back the full batch.

        :return: None.
        """
        self._create_source([("G-FIRST", "AAAAAA"), ("G-FAIL", "BBBBBB")])
        self._create_target([("G-FIRST", None), ("G-FAIL", None)])
        with closing(sqlite3.connect(self.target_path)) as connection, connection:
            connection.execute(
                """
                CREATE TRIGGER reject_failure
                BEFORE UPDATE OF address ON AIRCRAFT
                WHEN NEW.registration = 'G-FAIL'
                BEGIN
                    SELECT RAISE(ABORT, 'simulated update failure');
                END
                """
            )

        with self.assertRaises(ImportValidationError):
            import_addresses(self.source_path, self.target_path)

        self.assertEqual(
            [("G-FIRST", None), ("G-FAIL", None)], self._target_rows()
        )

    def test_rejects_missing_schema_and_same_database(self) -> None:
        """Invalid database configuration fails validation.

        :return: None.
        """
        with closing(sqlite3.connect(self.source_path)) as connection, connection:
            connection.execute("DROP TABLE AIRCRAFT")

        with self.assertRaises(ImportValidationError):
            import_addresses(self.source_path, self.target_path)
        with self.assertRaises(ImportValidationError):
            import_addresses(self.target_path, self.target_path)

    def test_command_line_paths_override_environment_paths(self) -> None:
        """Explicit CLI paths are used instead of environment defaults.

        :return: None.
        """
        self._create_source([("G-CLI", "ABCDEF")])
        self._create_target([("G-CLI", None)])
        output = io.StringIO()
        errors = io.StringIO()

        status = main(
            [
                "--source-db",
                str(self.source_path),
                "--target-db",
                str(self.target_path),
            ],
            {
                "AIRCRAFT_TRACKER_DB": "/does/not/exist",
                "FLIGHT_RECORDER_DB": "/also/missing",
            },
            output,
            errors,
        )

        self.assertEqual(0, status)
        self.assertIn("Updated: 1", output.getvalue())
        self.assertEqual("", errors.getvalue())

    def test_environment_paths_are_supported(self) -> None:
        """Environment variables provide paths when CLI options are omitted.

        :return: None.
        """
        self._create_source([("G-ENV", "ABCDEF")])
        self._create_target([("G-ENV", None)])

        status = main(
            [],
            {
                "AIRCRAFT_TRACKER_DB": str(self.source_path),
                "FLIGHT_RECORDER_DB": str(self.target_path),
            },
            io.StringIO(),
            io.StringIO(),
        )

        self.assertEqual(0, status)
        self.assertEqual([("G-ENV", "ABCDEF")], self._target_rows())

    def test_missing_path_returns_nonzero_status(self) -> None:
        """Absent configuration produces a concise command-line failure.

        :return: None.
        """
        errors = io.StringIO()

        status = main([], {}, io.StringIO(), errors)

        self.assertNotEqual(0, status)
        self.assertIn("AIRCRAFT_TRACKER_DB", errors.getvalue())

    def test_address_validation_requires_six_hexadecimal_characters(self) -> None:
        """Only unprefixed six-character hexadecimal values are accepted.

        :return: None.
        """
        self.assertEqual("A1B2C3", normalize_icao_address(" a1b2c3 "))
        self.assertIsNone(normalize_icao_address("0xA1B2C3"))
        self.assertIsNone(normalize_icao_address("ABCDE"))
        self.assertIsNone(normalize_icao_address("ZZZZZZ"))

    def test_resolve_database_path_rejects_missing_file(self) -> None:
        """Path resolution verifies that the selected file exists.

        :return: None.
        """
        with self.assertRaises(ImportValidationError):
            resolve_database_path(
                None, "SOURCE_DB", {"SOURCE_DB": "/missing/source.db"}, "Source"
            )

    def test_immutable_database_uri_is_explicitly_read_only(self) -> None:
        """Immutable source URIs retain read-only mode.

        :return: None.
        """
        uri = database_uri(self.source_path.resolve(), "ro", immutable=True)

        self.assertTrue(uri.endswith("?mode=ro&immutable=1"))


if __name__ == "__main__":
    unittest.main()
