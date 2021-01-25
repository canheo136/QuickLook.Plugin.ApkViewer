using System;
using System.Collections.Generic;

namespace AAPTForNet.Models {
    public class Icon {
        private const string defaultIcon = "ic_launcher.png";

        internal static readonly Icon Default = new Icon(defaultIcon);

        /// <summary>
        /// Return absolute path to package icon if @isImage is true,
        /// otherwise return empty string
        /// </summary>
        public string RealPath { get; set; }

        /// <summary>
        /// Determines whether icon of package is an image
        /// </summary>
        public bool isImage {
            get {
                return !defaultIcon.Equals(IconName) &&
                    !RealPath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);
            }
        }

        internal string IconName { get; }

        internal Icon() => throw new NotImplementedException();

        internal Icon(string iconName) {
            this.IconName = iconName;
            this.RealPath = string.Empty;
        }

        public override bool Equals(object obj) {
            if(obj is Icon ic) {
                return this.IconName == ic.IconName;
            }
            return false;
        }

        public override int GetHashCode() => -489061483 + EqualityComparer<string>.Default.GetHashCode(this.IconName);
    }
}
