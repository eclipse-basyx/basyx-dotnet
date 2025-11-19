@echo off

set "root=%~dp0"

for /r "%root%" %%d in (.) do (
    pushd "%%d"
    for /d %%i in (bin obj) do (
        if exist "%%i" (
            echo Deleting %%i in "%%d"
            rmdir /s /q "%%i"
        )
    )
    popd
)

IF %ERRORLEVEL% EQU 0 (
	exit 0
) ELSE (
	Echo One or more errors occured
	pause
)