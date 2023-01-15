using System.Collections.Generic;
using AAPTForNet.Models;

namespace AAPTForNet.Filters {
    internal class SDKFilter : BaseFilter {

        private List<string> msgs = new List<string>();
        private string[] segments => string.Join(" ", msgs).Split(seperator);

        public override bool CanHandle(string msg) {
            return msg.StartsWith("sdkVersion:") || msg.StartsWith("targetSdkVersion:");
        }

        public override void AddMessage(string msg) {
            if (!msgs.Contains(msg)) {
                msgs.Add(msg);
            }
        }

        public override ApkInfo GetAPK() {
            return new ApkInfo() {
                MinSDK = SDKInfo.GetInfo(GetMinSDKVersion()),
                TargetSDK = SDKInfo.GetInfo(GetTargetSDKVersion())
            };
        }

        public override void Clear() {
            msgs.Clear();
        }

        private string GetMinSDKVersion() {
            for (int i = 0; i < segments.Length; i++) {
                if (segments[i].Contains("sdkVersion"))
                    return segments[++i];
            }
            return string.Empty;
        }

        private string GetTargetSDKVersion() {
            for (var i = 0; i < segments.Length; i++) {
                if (segments[i].Contains("targetSdkVersion"))
                    return segments[++i];
            }
            return string.Empty;
        }
    }
}
