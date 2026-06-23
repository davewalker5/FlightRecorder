#!/bin/bash -f

REPORTS_ROOT=$( cd "$( dirname "$0" )" && pwd )

# Deactivate and remove the old virtual environment, if present
echo "Removing existing Virtual Environment, if present ..."
deactivate 2> /dev/null || true
rm -fr "$REPORTS_ROOT/venv"

# Create a new environment and activate it
echo "Creating new Virtual Environment ..."
python -m venv "$REPORTS_ROOT/venv"
. "$REPORTS_ROOT/venv/bin/activate"

# Make sure pip is up to date
pip install --upgrade pip

# Install the reporting suite dependencies
pip install -e "$REPORTS_ROOT"
