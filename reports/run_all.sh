#!/bin/bash -f

# Activate the virtual environment
export REPORTS_ROOT=$( cd "$( dirname "$0" )" && pwd )
. $REPORTS_ROOT/venv/bin/activate

# Suppress warnings about the output file extension
export PYTHONWARNINGS="ignore"

# Define a list of notebooks to skip
declare -a exclusions=(
)

# Get a list of Jupyter Notebooks and iterate over them
files=$(find `pwd` -name '*.ipynb')
while IFS= read -r file; do
    # Get the notebook file name and extension without the path
    filename=$(basename -- "$file")

    # See if the notebook is in the exclusions list
    found=0
    if [[ " ${exclusions[@]} " =~ " $filename " ]]; then
        found=1
    fi

    # If this notebook isn't in the exclusions list, run it
    if [[ found -eq 0 ]]; then
    echo $file
        papermill "$file" /dev/null
    fi
done <<< "$files"
