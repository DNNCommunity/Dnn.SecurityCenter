// MIT License
// Copyright DNN Community

using System;
using System.Collections.Generic;

namespace Dnn.Modules.SecurityCenter.ViewModels
{
    /// <summary>
    /// Represents the json response from the security service.
    /// </summary>
    internal class SecurityBulletinsReportViewModel
    {
        /// <summary>
        /// Gets or sets the list of security bulletins.
        /// </summary>
        public List<SecurityBulletinInfo> SecurityBulletins { get; set; }

        /// <summary>
        /// Represents a single security bulletin.
        /// </summary>
        public class SecurityBulletinInfo
        {
            /// <summary>
            /// Gets or sets the bulletin identifier.
            /// Example: 2022-12.
            /// </summary>
            public string Identifier { get; set; }

            /// <summary>
            /// Gets or sets the bulletin title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the bulletin summary (should be plain text).
            /// </summary>
            public string Summary { get; set; }

            /// <summary>
            /// Gets or sets the description of the bulletin (may contain encoded html).
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the date when the bulletin was published.
            /// </summary>
            public DateTime CreateDate { get; set; }

            /// <summary>
            /// Gets or sets a URL whic contains more details about the bulletin.
            /// </summary>
            public Uri DetailUrl { get; set; }
        }
    }
}
