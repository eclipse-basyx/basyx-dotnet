start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet clean basyx-applications\BaSyx.Applications.sln
dotnet build basyx-applications\BaSyx.Applications.sln --force
IF %ERRORLEVEL% EQU 0 (exit 0) ELSE (Echo One or more errors occured during compiling)