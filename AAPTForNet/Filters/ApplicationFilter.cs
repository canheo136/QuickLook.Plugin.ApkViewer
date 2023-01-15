using AAPTForNet.Models;

namespace AAPTForNet.Filters {
    internal class ApplicationFilter : BaseFilter {

        private string[] segments = new string[] { };

        public override bool CanHandle(string msg) {
            return msg.StartsWith("application:");
        }

        public override void AddMessage(string msg = "") {
            segments = msg.Split(seperator);
        }

        public override ApkInfo GetAPK() {
            // Try getting icon name from manifest, may be an image
            string iconName = GetValue("icon=");

            return new ApkInfo() {
                AppName = GetValue("label="),
                Icon = iconName == defaultEmptyValue ?
                    Icon.Default : new Icon(iconName)
            };
        }

        public override void Clear() {
            segments = new string[] { };
        }

        private string GetValue(string key) {
            string output = string.Empty;
            for (int i = 0; i < segments.Length; i++) {
                if (segments[i].Contains(key)) {
                    output = segments[++i];
                    break;
                }
            }
            return string.IsNullOrEmpty(output) ? defaultEmptyValue : output;
        }
    }
}
