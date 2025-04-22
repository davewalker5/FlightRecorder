# Flight Recorder Reporting

This folder contains Jupyter notebooks and supporting files for reporting on aircraft sightings stored in the Flight Recorder SQLite database. This provides more flexible reporting than the built-in reports in the application itself.

The following reports are currently available:

| Notebook | Report Type |
| --- | --- |
| aircraft_type_trends.ipynb | Monthly and yearly sightings by aircraft type |
| sightings_over_time.ipynb | Daily, monthly and yearly sightings and trends over time |
| manufacturer_aircraft_heatmap.ipynb | Heatmap of models by manufacturer for all manufacturers |
| manufacturer_aircraft_pie_chart.ipynb | Pie chart of model by sightings for a manufacturer |
| top_airlines_over_time.ipynb | Top 'N' airlines by month and year |
| top_manufacturers_and_models.ipynb | Top 'N' manufacturers and aircraft types |
| top_routes.ipynb | Top routes (unidirectional and bidirectional) |

## Setting Up the Reporting Environment

The reports have been written and tested using [Visual Studio Code](https://code.visualstudio.com/download) and the Jupyter extension from Microsoft using a Python virtual environment with the requirements listed in requirements.txt installed as the kernel for running the notebooks.

### Set Environment Variables

The following environment variable to be set *before* running code and opening the notebook:

``` bash
export FLIGHT_RECORDER_DB=/path/to/flightrecorder.db
```

Or, in PowerShell:

```powershell
$env: FLIGHT_RECORDER_DB = C:\path\to\flightrecorder.db
```

### Build the Virtual Environment

To build the virtual environment, run the following command:

```bash
./make_venv.sh
```

Or, in PowerShell:

```powershell
.\make_venv.bat
```

## Running a Report in Visual Studio Code

- Open the Jupyter notebook for the report of interest
- If using Visual Studio Code, select the Python virtual environment as the kernel for running the notebook
- Review the instructions at the top of the report and make any required changes to e.g. reporting parameters
- Click on "Run All" to run the report and export the results
- Exported results are written to a folder named "exported" within the reports folder
