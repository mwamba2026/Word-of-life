@echo off
setlocal
cd /d "%~dp0"

if "%ASPNETCORE_ENVIRONMENT%"=="" set ASPNETCORE_ENVIRONMENT=Development

if "%ConnectionStrings__DefaultConnection%"=="" (
  echo Set ConnectionStrings__DefaultConnection to your MySQL connection string before starting.
  exit /b 1
)

curl.exe -fsS http://localhost:5000/api/health >nul 2>&1
if not errorlevel 1 goto open_site

if not exist "publish\FaithOutreach.Api.exe" (
  call "%~dp0Publish Words of Life.cmd"
  if errorlevel 1 exit /b 1
)

start "Words of Life" /min /D "%~dp0publish" FaithOutreach.Api.exe --urls http://localhost:5000

echo Starting Words of Life...
timeout /t 4 /nobreak >nul

:open_site
start "" http://localhost:5000

endlocal
