// MIT License
// Copyright DNN Community

using System;
using System.Collections.Generic;

namespace Dnn.Modules.SecurityCenter.ViewModels
{
    /// <summary>
    /// A viewmodel that represents DNN Security Bulletins.
    /// </summary>
    public class SecurityBulletinsViewModel
    {
        /// <summary>
        /// Gets or sets the title of the RSS feed.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the url to download DNN Platform.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the RSS feed description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the list of security bulletins.
        /// </summary>
        public IEnumerable<Bulletin> Bulletins { get; set; }

        /// <summary>
        /// Represents a single DNN Security Bulletin.
        /// </summary>
        public class Bulletin
        {
            /// <summary>
            /// Gets or sets a link to the detailed security bulletin.
            /// </summary>
            public string Link { get; set; }

            /// <summary>
            /// Gets or sets the title of the bulletin.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the short description of the bulletin.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets a string representing the date of announcement.
            /// </summary>
            public DateTime PublicationDateUtc { get; set; }
        }
    }
}
