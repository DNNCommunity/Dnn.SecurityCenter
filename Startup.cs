// MIT License
// Copyright DNN Community

using Dnn.Modules.SecurityCenter.Data;
using Dnn.Modules.SecurityCenter.Data.Entities;
using Dnn.Modules.SecurityCenter.Data.Repositories;
using Dnn.Modules.SecurityCenter.Services;
using DotNetNuke.DependencyInjection;
using DotNetNuke.Services.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Dnn.Modules.SecurityCenter
{
    /// <summary>
    /// Implements the IDnnStartup interface to run at application start.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup : IDnnStartup
    {
        /// <summary>
        /// Registers the dependencies for injection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ModuleDbContext, ModuleDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IItemService>(provider => new ItemService(provider.GetService<IRepository<Item>>()));
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddSingleton<ISecurityService, SecurityService>();
        }
    }
}