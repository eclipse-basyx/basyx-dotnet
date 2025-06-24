dotnet clean BaSyx.sln
dotnet build -c Debug BaSyx.sln --force
IF NOT %ERRORLEVEL% EQU 0 (
	Echo One or more errors occured during compiling
	pause
)
exit 0