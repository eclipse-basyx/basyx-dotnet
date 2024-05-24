# Welcome to basyx-examples!
In this repository you'll find some examples regarding building and hosting Asset Administration Shells and Submodels.

Some of them are in combination with Repositories that store the Asset Administration Shells resp. Submodels. Some of them provide a stand-alone server version of Asset Administration Shells and/or Submodels only. Some of them are mixtures of all of the components.

And all of those server applications come with a magnificient user interface reachable at 

> http(s)://YOUR_IP_ADDRESS:YOUR_PORT/ui

and of course with an OpenAPI documentation at

> http(s)://YOUR_IP_ADDRESS:YOUR_PORT/swagger

## HelloAssetAdministrationShell
You always start with a HelloWorld, don't you? The *HelloAssetAdministrationShell* is a good starting point to get used to the concept of how to build an Asset Administration Shell and host it as an HTTP server. Additionally, it provides insights to the concept of *ServiceProvider*s for Asset Administration Shell and Submodels and how they interact with each other.

## SimpleAssetAdministrationShell
Wow, it's getting even more simple? Yes! The *SimpleAssetAdministrationShell* provides you an powerful example of how fast and efficient you can create and host an Asset Administration Shell and a separately hosted Submodel which in turn is linked from the Asset Administration Shell.

## MultiAssetAdministrationShell
And what if I have a bunch of Asset Administration Shell? There ya go! The MultiAssetAdministrationShell examples shows how to create and host a couple of Asset Administration Shells at the same time and place. It is stored in an Asset Administration Shell Repository that provides a server API as well.  And for all of that, you won't even leave *Program.cs*. I know, it's insane.

## ComplexAssetAdministration
Speaking of insanity, the ComplexAssetAdministrationShell example will knock your socks off! It...

 1. initializes and starts a local file-based Registry,
 2. creates and hosts a stand-alone Asset Administration Shell,
 3. creates and hosts multiple Asset Administration Shell residing in a Repository,
 4. creates and hosts multiple Submodels residing in a Submodel Repository,
 5. and brings all that stuff together via respective client instances.

And again you won't even leave *Program.cs*. Isn't it beautiful?

