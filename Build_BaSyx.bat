IF "%BASYX_REPO%"=="" (
	start /w Setup_BaSyx.bat
)

ECHO Using BaSyx-Package-Source: %BASYX_REPO%

start /w Clear_bin_obj_Folders.bat
start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet clean BaSyx.sln
dotnet build -c Release BaSyx.sln --force
IF NOT %ERRORLEVEL% EQU 0 (
	Echo One or more errors occured during compiling
	pause
)
exit 0