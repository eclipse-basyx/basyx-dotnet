start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet clean ..\..\basyx-components\BaSyx.Components.sln
dotnet build -c Release ..\..\basyx-components\BaSyx.Components.sln --force
IF %ERRORLEVEL% EQU 0 (exit 0) ELSE (Echo One or more errors occured during compiling)