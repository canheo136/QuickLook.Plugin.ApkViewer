namespace AAPTForNet {
    public class SDKInfo {
        public static readonly SDKInfo Unknown = new SDKInfo("0", "0", "0");

        private static readonly string[] AndroidCodeName = {
            "Unknown",

            "Cupcake",  // API level 3
            "Donut",
            "Eclair",
            "Eclair",
            "Eclair",
            "Froyo",
            "Gingerbread",
            "Gingerbread",
            "Honeycomb",
            "Honeycomb",
            "Honeycomb",
            "Ice Cream Sandwich",
            "Ice Cream Sandwich",
            "Jelly Bean",
            "Jelly Bean",
            "Jelly Bean",
            "KitKat",
            "Unknown",  // API level 20
            "Lollipop",
            "Lollipop",
            "Marshmallow",
            "Nougat",
            "Nougat",
            "Oreo",
            "Oreo",
            "Pie",
            "Android10"  // API level 29
        };

        private static readonly string[] AndroidVersionCode = {
            "Unknown",

            "1.5",  // API level 3
            "1.6",
            "2.0",
            "2.0",
            "2.1",
            "2.2",
            "2.3",
            "2.3",
            "3.0",
            "3.1",
            "3.2",
            "4.0",
            "4.0",
            "4.1",
            "4.2",
            "4.3",
            "4.4",
            "Unknown",  // API level 20
            "5.0",
            "5.1",
            "6.0",
            "7.0",
            "7.1",
            "8.0",
            "8.1",
            "9",
            "10"    // API level 29
        };

        public string APILever { get; }
        public string Version { get; }
        public string CodeName { get; }

        protected SDKInfo(string level, string ver, string code) {
            this.APILever = level;
            this.Version = ver;
            this.CodeName = code;
        }

        public static SDKInfo GetInfo(int sdkVer) {
            // Must be update if android releases new version
            int index = (sdkVer < 3 || sdkVer > 29) ? 0 : sdkVer - 2;

            return new SDKInfo(sdkVer.ToString(), AndroidVersionCode[index], AndroidCodeName[index]);
        }

        public static SDKInfo GetInfo(string sdkVer) {
            int.TryParse(sdkVer, out int ver);

            return GetInfo(ver);
        }

        public override string ToString() {
            if (APILever.Equals("0") && Version.Equals("0") && CodeName.Equals("0"))
                return AndroidCodeName[0];

            return $"API Level {this.APILever} " +
                $"{(this.Version == AndroidCodeName[0] ? $"({AndroidCodeName[0]} - " : $"(Android {this.Version} - ")}" +
                $"{this.CodeName})";
        }
    }
}
