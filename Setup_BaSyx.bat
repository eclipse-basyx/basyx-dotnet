@ECHO OFF
SET currentPath=%~dp0
echo New NuGet-Repository Path: %currentPath:~0,-1%\basyx-dotnet-nuget-packages
setx BASYX_REPO %currentPath:~0,-1%\basyx-dotnet-nuget-packages
mkdir basyx-dotnet-nuget-packages
dotnet nuget remove source BaSyx.Packages
dotnet nuget add source %currentPath:~0,-1%\basyx-dotnet-nuget-packages --name BaSyx.Packages
IF %ERRORLEVEL% EQU 0 (
	exit 0
) ELSE (
	Echo One or more errors occured
	pause
)