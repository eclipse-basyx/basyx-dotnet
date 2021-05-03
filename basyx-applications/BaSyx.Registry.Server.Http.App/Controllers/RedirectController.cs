using BaSyx.API.Components;
using BaSyx.API.Http.Controllers;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Utils.Network;
using BaSyx.Utils.ResultHandling;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BaSyx.Registry.Server.Http.App.Controllers
{
    public class RedirectController : Controller
    {
        private readonly IAssetAdministrationShellRegistry serviceProvider;

        /// <summary>
        /// The constructor for the Asset Administration Shell Redirect Controller
        /// </summary>
        /// <param name="aasRegistry">The backend implementation for the IAssetAdministrationShellRegistry interface. Usually provided by the Depedency Injection mechanism.</param>
        public RedirectController(IAssetAdministrationShellRegistry aasRegistry)
        {
            serviceProvider = aasRegistry;
        }


        /// <summary>
        /// Redirects (302) to the first reachable endpoint of Asset Administration Shell registered
        /// </summary>
        /// <param name="aasId">The Asset Administration Shell's unique id</param>
        /// <param name="toWhat">The path at the found endpoint</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested Asset Administration Shell</response>
        /// <response code="400">Bad Request</response> 
        /// <response code="404">No Asset Administration Shell with passed id found</response>     
        [HttpGet("api/v1/registry/{aasId}/redirect/{toWhat}", Name = "RedirectToAssetAdministrationShell")]
        public async Task<IActionResult> RedirectToAssetAdministrationShell(string aasId, string toWhat)
        {
            if (string.IsNullOrEmpty(aasId))
                return ResultHandling.NullResult(nameof(aasId));

            aasId = HttpUtility.UrlDecode(aasId);
            var result = serviceProvider.RetrieveAssetAdministrationShellRegistration(aasId);

            if(!result.Success)
                return result.CreateActionResult(CrudOperation.Retrieve);

            try
            {
                IAssetAdministrationShellDescriptor descriptor = result.Entity;
                foreach (var endpoint in descriptor.Endpoints.OfType<HttpEndpoint>())
                {
                    bool pingable = await NetworkUtils.PingHostAsync(endpoint.Url.Host);
                    if (pingable)
                    {
                        return Redirect(endpoint.Address.Replace("/aas", "/" + toWhat));
                    }
                }

                result.Messages.Add(new Message(MessageType.Error, "Endpoints are not reachable"));
            }
            catch (Exception e)
            {
                var tempResult = new Result(e);
                result.Messages.AddRange(tempResult.Messages);
            }
            return new BadRequestObjectResult(result);
        }
    }
}
