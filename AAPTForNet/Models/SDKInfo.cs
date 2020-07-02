namespace AAPTForNet {
    public class SDKInfo {
        public static readonly SDKInfo Unknown = new SDKInfo("0", "0", "0");

        private static readonly string[] AndroidCodeName = {
            "Unknown",

            "Cupcake", // 3
            "Donut", // 4
            "Eclair", "Eclair", "Eclair", // 5, 6, 7
            "Froyo", // 8
            "Gingerbread", "Gingerbread", // 9, 10
            "Honeycomb", "Honeycomb", "Honeycomb", // 11, 12, 13
            "Ice Cream Sandwich", "Ice Cream Sandwich", // 14, 15
            "Jelly Bean", "Jelly Bean", "Jelly Bean",   // 16, 17, 18
            "KitKat",   // 19
            "Unknown",  // 20
            "Lollipop", "Lollipop", //  21, 22
            "Marshmallow",  // 23
            "Nougat", "Nougat", // 24, 25
            "Oreo", "Oreo", // 26, 27
            "Pie",  // 28
        };

        private static readonly string[] AndroidVersionCode = {
            "Unknown",

            "1.5",  // 3
            "1.6",  // 4
            "2.0", "2.0",  // 5, 6
            "2.1",  // 7
            "2.2",  // 8
            "2.3", "2.3",  // 9, 10
            "3.0",  // 11
            "3.1",  // 12
            "3.2",  // 13
            "4.0", "4.0",   // 14, 15
            "4.1",  // 16
            "4.2",  // 17
            "4.3",  // 18
            "4.4",  // 19
            "Unknown",  // 20
            "5.0",  // 21
            "5.1",  // 22
            "6.0",  // 23
            "7.0",  // 24
            "7.1",  // 25
            "8.0",  // 26
            "8.1",  // 27
            "9",    // 28
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
            int index = (sdkVer < 14 || sdkVer > 28) ? 0 : sdkVer - 13;

            return new SDKInfo(sdkVer.ToString(), AndroidVersionCode[index], AndroidCodeName[index]);
        }

        public static SDKInfo GetInfo(string sdkVer) {
            int.TryParse(sdkVer, out int ver);

            return GetInfo(ver);
        }

        public override string ToString() {
            if (APILever.Equals("0") && Version.Equals("0") && CodeName.Equals("0"))
                // Empty SDK
                return "Unknown";

            return $"API Level {this.APILever} " +
                $"{(this.Version == "Unknown" ? "(Unknown - " : $"(Android {this.Version} - ")}" +
                $"{this.CodeName})";
        }
    }
}
