dotnet clean basyx-dotnet-sdk\BaSyx.Core.sln
dotnet build -c Debug basyx-dotnet-sdk\BaSyx.Core.sln --force
IF NOT %ERRORLEVEL% EQU 0 (
	Echo One or more errors occured during compiling
	pause
)

dotnet clean basyx-dotnet-components\BaSyx.Components.sln
dotnet build -c Debug basyx-dotnet-components\BaSyx.Components.sln --force
IF NOT %ERRORLEVEL% EQU 0 (
	Echo One or more errors occured during compiling
	pause
)
exit 0