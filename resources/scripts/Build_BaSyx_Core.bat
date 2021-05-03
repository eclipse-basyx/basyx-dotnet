start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet clean ..\..\basyx-core\BaSyx.Core.sln
dotnet build -c Release ..\..\basyx-core\BaSyx.Core.sln --force
IF %ERRORLEVEL% EQU 0 (exit 0) ELSE (Echo One or more errors occured during compiling)