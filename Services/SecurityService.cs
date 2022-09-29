// MIT License
// Copyright DNN Community

using Dnn.Modules.SecurityCenter.ViewModels;
using DotNetNuke.Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace Dnn.Modules.SecurityCenter.Services
{
    /// <summary>
    /// Provides access to the DNN security service at dnnplatform.io.
    /// </summary>
    internal class SecurityService : ISecurityService
    {
        /// <inheritdoc/>
        public async Task<SecurityBulletinsViewModel> GetAllSecurityBulletinsAsync(string versionString)
        {
            return await Task.Run(async () =>
            {
                var cacheKey = $"Dnn.Security_SecurityBelletinsViewModel_{versionString}";
                var cached = DataCache.GetCache<SecurityBulletinsViewModel>(cacheKey);
                if (cached is null)
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync($"https://dnnplatform.io/security/vulnerabilitiesjson/{versionString}");
                        response.EnsureSuccessStatusCode();
                        var bulletinsString = await response.Content.ReadAsStringAsync();
                        var bulletins = new SecurityBulletinsReportViewModel
                        {
                            SecurityBulletins = JsonConvert.DeserializeObject<List<SecurityBulletinsReportViewModel.SecurityBulletinInfo>>(bulletinsString),
                        };

                        var viewModel = new SecurityBulletinsViewModel
                        {
                            Bulletins = bulletins.SecurityBulletins.Select(b => new SecurityBulletinsViewModel.Bulletin
                            {
                                Description = b.Description,
                                Link = b.DetailUrl.ToString(),
                                PublicationDateUtc = b.CreateDate,
                                Title = b.Title,
                            }),
                        };

                        DataCache.SetCache(cacheKey, viewModel, DateTime.Now.AddHours(1));
                        return viewModel;
                    }
                }

                return cached;
            });
        }
    }
}
