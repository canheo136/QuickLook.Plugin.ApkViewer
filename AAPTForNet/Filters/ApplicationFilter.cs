using AAPTForNet.Models;

namespace AAPTForNet.Filters {
    internal class ApplicationFilter : BaseFilter {

        private string[] segments = new string[] { };

        public override bool canHandle(string msg) {
            return msg.StartsWith("application:");
        }

        public override void addMessage(string msg = "") {
            segments = msg.Split(seperator);
        }

        public override ApkInfo getAPK() {
            return new ApkInfo() {
                AppName = getName()
            };
        }

        public override void clear() => segments = new string[] { };

        private string getName() {
            for(int i = 0; i < segments.Length; i++) {
                if (segments[i].Contains("label=")) // Find key
                    return segments[++i];           // Return value
            }
            return string.Empty;
        }
    }
}
