@echo off
setlocal
cd /d "%~dp0"

if not exist "frontend\node_modules" (
  pushd "frontend"
  call npm.cmd ci
  if errorlevel 1 exit /b 1
  popd
)

pushd "frontend"
call npm.cmd run build
if errorlevel 1 exit /b 1
popd

dotnet publish "backend\FaithOutreach.Api\FaithOutreach.Api.csproj" -c Release -o "publish"
if errorlevel 1 exit /b 1

echo.
echo Publish completed successfully in:
echo %~dp0publish
echo.
echo Run publish\FaithOutreach.Api.exe, then open http://localhost:5000

endlocal
