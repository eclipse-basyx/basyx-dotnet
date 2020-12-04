@ECHO OFF
SET currentPath=%~dp0
echo New NuGet-Repository Path: %currentPath\basyx-packages
setx BASYX_REPO %currentPath:~0,-1%\basyx-packages
mkdir basyx-packages
nuget sources add -name BaSyx.Packages -source %currentPath:~0,-1%\basyx-packages