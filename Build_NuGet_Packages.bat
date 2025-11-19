@echo off
setlocal

SET currentPath=%~dp0
echo New Enviroment Variable 'BASYX_SDK': %currentPath:~0,-1%
setx BASYX_SDK %currentPath:~0,-1%
mkdir nuget-packages

:: Prompt for version
set /p VERSION=Enter Version Tag (e.g., 1.0.0): 

start /w Clear_bin_obj_Folders.bat
start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet restore BaSyx.sln
dotnet build BaSyx.sln --configuration Release --no-restore
dotnet pack BaSyx.sln --configuration Release --no-build --include-source --include-symbols --output nuget-packages -p:PackageVersion=%VERSION% -p:Version=%VERSION%

IF NOT %ERRORLEVEL% EQU 0 (
	Echo One or more errors occured during compiling
	pause
)