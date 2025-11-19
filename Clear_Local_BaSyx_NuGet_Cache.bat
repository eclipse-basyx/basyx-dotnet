cd /D %USERPROFILE%\.nuget\packages
for /f %%i in ('dir /a:d /s /b basyx.*') do rd /s /q %%i

IF %ERRORLEVEL% EQU 0 (
	exit 0
) ELSE (
	Echo One or more errors occured
	pause
)