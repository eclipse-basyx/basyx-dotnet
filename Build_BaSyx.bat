start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet build basyx-core\BaSyx.Core.sln --force

start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet build basyx-components\BaSyx.Components.sln --force

start /w Clear_Local_BaSyx_NuGet_Cache.bat

dotnet build basyx-examples\BaSyx.Examples.sln --force