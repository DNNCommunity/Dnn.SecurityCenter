// MIT License
// Copyright DNN Community

using Dnn.Modules.SecurityCenter.ViewModels;
using System.Threading.Tasks;

namespace Dnn.Modules.SecurityCenter.Services
{
    /// <summary>
    /// Provides access to the DNN security service at dnnplatform.io.
    /// </summary>
    public interface ISecurityService
    {
        /// <summary>
        /// Gets a list of all security bulletins that apply for a given version.
        /// </summary>
        /// <param name="versionString">A string representation of a DNN version in the format 090202 for v9.2.2.</param>
        /// <returns><see cref="SecurityBulletinsViewModel"/>.</returns>
        Task<SecurityBulletinsViewModel> GetAllSecurityBulletinsAsync(string versionString);
    }
}