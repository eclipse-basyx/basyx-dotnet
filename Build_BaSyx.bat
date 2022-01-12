IF "%BASYX_REPO%"=="" OR IF NOT EXIST basyx-dotnet-nuget-packages\NUL (
	start /w Setup_BaSyx.bat
)

ECHO Using BaSyx-Package-Source: %BASYX_REPO%

start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet clean basyx-dotnet-sdk\BaSyx.Core.sln
dotnet build -c Release basyx-dotnet-sdk\BaSyx.Core.sln --force
IF NOT %ERRORLEVEL% EQU 0 (
	Echo One or more errors occured during compiling
	pause
)

dotnet clean basyx-dotnet-components\BaSyx.Components.sln
dotnet build -c Release basyx-dotnet-components\BaSyx.Components.sln --force
IF NOT %ERRORLEVEL% EQU 0 (
	Echo One or more errors occured during compiling
	pause
)
exit 0