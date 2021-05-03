IF "%BASYX_REPO%"=="" start /w Setup_BaSyx.bat

ECHO Using BaSyx-Package-Source: %BASYX_REPO%

cd resources\scripts

start /w Build_BaSyx_Core.bat

start /w Build_BaSyx_Components.bat

start /w Build_BaSyx_Applications.bat