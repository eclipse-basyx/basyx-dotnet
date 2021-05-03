@ECHO OFF
SET currentPath=%~dp0
echo New NuGet-Repository Path: %currentPath:~0,-1%\basyx-packages
setx BASYX_REPO %currentPath:~0,-1%\basyx-packages
mkdir basyx-packages
dotnet nuget add source %currentPath:~0,-1%\basyx-packages --name BaSyx.Packages