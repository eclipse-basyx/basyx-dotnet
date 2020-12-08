# Welcome to basyx
In this repository you'll find the entire .NET stack of the BaSyx SDK.

## Build
1. Execute **Setup_BaSyx_NuGet_Repo.bat** only once and for all
2. Code as you wish, if you like to modify or extend the functionalities of the SDK
3. Execute **Build_BaSyx.bat** to build the entire SDK. 

It will generate NuGet Packages under the folder created in Setup_BaSyx_NuGet_Repo.bat called **basyx-packages**. Those packages are now available in the NuGet Package Manager in Visual Studio and to the dotnet CLI as NuGet source.

## Use
Cannot be easier - just use the NuGet Packages in Visual Studio. Don't forget to  initialize the NuGet package source with the help of **Setup_BaSyx_NuGet_Repo.bat**

## Troubleshoot
Sometimes or even more than sometimes Visual Studio uses still the old NuGet package version (if the version number is not incremented). Execute **Clear_Local_BaSyx_NuGet_Cache.bat** to solve that issue - it clears the local NuGet cache in your user's directory.
