# Welcome to the BaSyx .NET SDK

This is the the one and only repository to start workign with the BaSyx .NET SDK.

It implements the offical AAS Part 1: Metamodel v3 as well as the Part 2: API v1

The entire .NET SDK is structured in 5 separate folder:
- basyx-dotnet-sdk: Contains all the core libraries to build everything from scratch
- basyx-dotnet-components: Built on top of the core libraries providing more high-level components as well as various client and server libraries
- basyx-dotnet-applications: Off-the-shelf components ready to be used and deployed in any scenario (Cloud, On Premises, Embedded Systems, RaspberryPi, Docker, etc.)
- basyx-dotnet-examples: Solution with a couple of example projects showing how things work
- basyx-dotnet-test: Unit and integration tests of the SDK

# Setup BaSyx

## NuGet Packages
All tagged/released packages are available as NuGet packages on nuget.org and can be installed via NuGet Package Manager within Visual Studio.

## Build NuGet Packages on your own
In order to build your own NuGet packages and use them in your project with the newest commits on the main-branch clone or download this repository, then execute **Setup_BaSyx.bat** and **Build_BaSyx.bat** afterwards. 
Make sure Visual Studio is closed the first time you run these scripts. The first script will add a new folder **basyx-dotnet-nuget-packages** and add this folder as NuGet package source to the system. The second script will build the NuGet packages and put them into the mentioned folder. (To build the NuGet packages in Visual Studio don't forget to change the Solution Configuration to *Release*)
