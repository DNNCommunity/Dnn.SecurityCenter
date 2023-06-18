//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.

//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dnn.Modules.SecurityCenter.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A viewmodel that exposes all resource keys in strong types.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LocalizationViewModel
    {
        /// <summary>
        /// Localized strings present the ModelValidation resources.
        /// </summary>
        public ModelValidationInfo ModelValidation { get; set; }

        /// <summary>
        /// Localized strings present the UI resources.
        /// </summary>
        public UIInfo UI { get; set; }
        /// <summary>
        /// Localized strings for the ModelValidation resources.
        /// </summary>
        public class ModelValidationInfo
        {
            /// <summary>Gets or sets the IdGreaterThanZero localized text.</summary>
            /// <example>The Id must be an integer bigger than 0</example>
            public string IdGreaterThanZero { get; set; }

            /// <summary>Gets or sets the NameRequired localized text.</summary>
            /// <example>The name is required</example>
            public string NameRequired { get; set; }


        }

        /// <summary>
        /// Localized strings for the UI resources.
        /// </summary>
        public class UIInfo
        {
            /// <summary>Gets or sets the DnnPlatformVersion localized text.</summary>
            /// <example>DNN Platform Version</example>
            public string DnnPlatformVersion { get; set; }

            /// <summary>Gets or sets the DnnSecurityCenter localized text.</summary>
            /// <example>DNN Security Center</example>
            public string DnnSecurityCenter { get; set; }

            /// <summary>Gets or sets the Loading localized text.</summary>
            /// <example>Loading...</example>
            public string Loading { get; set; }

            /// <summary>Gets or sets the NoBulletins localized text.</summary>
            /// <example>There are no known security vulnerabilities for the selected DNN Platform version.</example>
            public string NoBulletins { get; set; }


        }


    }
}
