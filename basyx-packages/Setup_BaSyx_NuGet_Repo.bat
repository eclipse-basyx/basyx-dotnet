@ECHO OFF
SET currentPath=%~dp0
echo New NuGet-Repository Path: %currentPath:~0,-1%
setx BASYX_REPO %currentPath:~0,-1%
dotnet nuget add source %currentPath:~0,-1% --name BaSyx.Packages