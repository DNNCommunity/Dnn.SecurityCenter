// MIT License
// Copyright DNN Community

using Dnn.Modules.SecurityCenter.ViewModels;
using DotNetNuke.Common.Utilities;
using System;
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
                    var client = new HttpClient();
                    var response = await client.GetStringAsync($"https://dnnplatform.io/security.aspx?type=Framework&name=DNNCorp.CE&version={versionString}");
                    var reader = XmlReader.Create(new StringReader(response));

                    SyndicationFeed feed = SyndicationFeed.Load(reader);

                    var viewModel = new SecurityBulletinsViewModel
                    {
                        Bulletins = feed.Items.Select(item => new SecurityBulletinsViewModel.Bulletin
                        {
                            Description = item.Summary.Text,
                            Link = item.Links.FirstOrDefault().Uri.ToString(),
                            PublicationDateUtc = item.PublishDate.UtcDateTime,
                            Title = item.Title.Text,
                        }),
                        Description = feed.Description.Text,
                        Link = feed.Links.FirstOrDefault().Uri.ToString(),
                        Title = feed.Title.Text,
                    };

                    DataCache.SetCache(cacheKey, viewModel, DateTime.Now.AddHours(1));
                    return viewModel;
                }

                return cached;
            });
        }
    }
}
