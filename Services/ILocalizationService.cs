// MIT License
// Copyright DNN Community
using Dnn.Modules.SecurityCenter.ViewModels;

namespace Dnn.Modules.SecurityCenter.Services
{
    /// <summary>
    /// Provides strongly typed localization services for this module.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets viewmodel that strongly types all resource keys.
        /// </summary>
        LocalizationViewModel ViewModel { get; }
    }
}