cd C:\Users\%USERNAME%\.nuget\packages
for /f %%i in ('dir /a:d /s /b basyx.*') do rd /s /q %%i
exit