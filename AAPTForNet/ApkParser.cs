using System.Text.RegularExpressions;

namespace AAPTForNet {
    /// <summary>
    /// Parse all messages from AAPTool
    /// </summary>
    internal class ApkParser {

        private static readonly string SDKPattern = @"sdkVersion:'[0-9]{0,1}[^']";
        private static readonly string IconPattern = @"icon='[\w-.\\\/]*[^']";
        private static readonly string AppNamePattern = @"label='[\w-.() \\\/\[\]]*[^']";
        private static readonly string PackagePattern = @"name='[\w.]*[^']";
        private static readonly string VersionPattern = @"versionName='[\w-.() \/]*[^']";
        protected ApkParser() { }

        public static ApkInfo Parse(DumpModel model) {
            if(!model.isSuccess)
                return new ApkInfo("", string.Join("\n", model.Messages), "", "", SDKInfo.Unknown, "");

            // Extract info from apk
            string name = "", package = "", ver = "", sdk = "", icon = "";
            
            foreach(string msg in model.Messages) {
                if(msg.StartsWith("application:")) {
                    icon = splitValueFromString(msg, IconPattern);
                    name = splitValueFromString(msg, AppNamePattern);
                }
                else if(msg.StartsWith("package:")) {
                    ver = splitValueFromString(msg, VersionPattern);
                    package = splitValueFromString(msg, PackagePattern);
                }
                else if(msg.StartsWith("sdkVersion:")) {
                    sdk = splitValueFromString(msg, SDKPattern);
                }
                else continue;
            }
            
            return new ApkInfo(model.FilePath, name, package, ver, SDKInfo.GetInfo(sdk), icon);
        }

        private static string splitValueFromString(string source, string pattern) {
            string temp = Regex.Match(source, pattern).Value;
            return temp.Substring(temp.IndexOf("'") + 1);
        }
    }
}
