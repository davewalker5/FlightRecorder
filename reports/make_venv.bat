@ECHO OFF

REM Deactivate and remove the old virtual environment, if present
ECHO Removing existing Virtual Environment, if present ...
deactivate > nul 2>&1
RMDIR /S /Q venv

REM Create a new environment and activate it
ECHO Creating new Virtual Environment ...
python -m venv venv
CALL venv\Scripts\activate.bat

REM Make sure pip is up to date
python -m pip install --upgrade pip

REM Install the requirements
python -m pip install -r requirements.txt

ECHO ON
