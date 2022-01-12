# Welcome to basyx-packages!
In this repository you'll find all ready-to-use NuGet packages of the entire BaSyx .NET SDK. With these packages you're ready to develop, e.g.:
 - Server Applications for 
	 - Asset Administration Shells & their optional Repositories, 
	 - Submodels & their optional Repositories, 
	 - Registry
 - Client Applications for
	 - Asset Administration Shells, 
	 - Submodels, 
	 - Registry.
 - Middleware Components 
	 - Registry
	 - mDNS-Discovery
 - Read & Write
	 - JSON & XML serialization
	 - AASX-Packages
 - and much more...

# Setup
In order to access the NuGet-Packages from the dotnet environment resp. Microsoft Visual Studio, the packages folder needs to be registered as NuGet source. 

Either use

> **Setup_BaSyx_NuGet_Repo.bat**

or open command prompt in the packages folder an enter

    setx BASYX_REPO %cd%
    dotnet nuget add source %cd% --name BaSyx.Packages

The first command sets an user environment variable. It's necessary if you want to build the SDK on your own. Each SDK project file contains a command as post-build process that generates a new NuGet package which will be stored in the folder mentioned in *BASYX_REPO* environment variable.
