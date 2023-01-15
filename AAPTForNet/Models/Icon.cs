using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AAPTForNet.Models {
    public class Icon {

        private const int hdpiWidth = 72;
        public const string DefaultName = "ic_launcher.png";

        internal static readonly Icon Default = new Icon(DefaultName);

        /// <summary>
        /// Return absolute path to package icon if @isImage is true,
        /// otherwise return empty string
        /// </summary>
        public string RealPath { get; set; }

        /// <summary>
        /// Determines whether icon of package is an image
        /// </summary>
        public bool IsImage => !DefaultName.Equals(IconName) && !IsMarkup;

        internal bool IsMarkup => IconName
            .EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

        // Not real icon, it refer to another
        internal bool IsRefernce => IconName.StartsWith("0x");

        internal bool IsHighDensity {
            get {
                if (!IsImage || !File.Exists(RealPath))
                    return false;

                try {
                    // Load from unsupported format will throw an exception.
                    // But icon can be packed without extension
                    using (var image = new Bitmap(RealPath)) {
                        return image.Width > hdpiWidth;
                    }
                }
                catch {
                    return false;
                }
            }
        }

        /// <summary>
        /// Icon name can be an asset image (real icon image),
        /// markup file (actually it's image, but packed to xml)
        /// or reference to another
        /// </summary>
        internal string IconName { get; set; }

        internal Icon() => throw new NotImplementedException();

        internal Icon(string iconName) {
            IconName = iconName;
            RealPath = string.Empty;
        }

        public override string ToString() {
            return IconName;
        }

        public override bool Equals(object obj) {
            if (obj is Icon ic) {
                return IconName == ic.IconName;
            }
            return false;
        }

        public override int GetHashCode() {
            return -489061483 + EqualityComparer<string>.Default.GetHashCode(IconName);
        }
    }
}
