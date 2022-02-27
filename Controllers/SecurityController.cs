// MIT License
// Copyright DNN Community

using Dnn.Modules.SecurityCenter.Services;
using Dnn.Modules.SecurityCenter.ViewModels;
using DotNetNuke.Web.Api;
using NSwag.Annotations;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Dnn.Modules.SecurityCenter.Controllers
{
    /// <summary>
    /// Provides information about DNN security services.
    /// </summary>
    public class SecurityController : ModuleApiController
    {
        private readonly ISecurityService securityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityController"/> class.
        /// </summary>
        /// <param name="securityService">Provides access to DNN security bulletins service.</param>
        public SecurityController(ISecurityService securityService)
        {
            this.securityService = securityService;
        }

        /// <summary>
        /// Gets all the DNN security bulletins.
        /// </summary>
        /// <param name="versionString">The version for which to get the security bulletins for in the format 090202 for v9.9.2.</param>
        /// <returns>A list of DNN security bulletins.</returns>
        [ValidateAntiForgeryToken]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = DotNetNuke.Security.SecurityAccessLevel.View)]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, typeof(SecurityBulletinsViewModel))]
        public async Task<IHttpActionResult> GetSecurityBulletins(string versionString)
        {
            try
            {
                var viewModel = await this.securityService.GetAllSecurityBulletinsAsync(versionString);
                return this.Ok(viewModel);
            }
            catch (HttpException)
            {
                return this.InternalServerError(new Exception("The update service is currently not available."));
                throw;
            }
        }
    }
}
