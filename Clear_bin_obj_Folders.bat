@echo off
setlocal

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

endlocal