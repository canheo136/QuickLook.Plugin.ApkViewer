using System.Text.RegularExpressions;

namespace AAPTForNet {
    /// <summary>
    /// Parse output messages from AAPTool
    /// </summary>
    internal class ApkParser {

        protected ApkParser() { }

        public static ApkInfo Parse(DumpModel model) {
            if (!model.isSuccess)
                return ApkInfo.Empty;

            var permissions = new System.Collections.Generic.List<string>();
            var supportScrs = new System.Collections.Generic.List<string>();

            string ver = string.Empty,
                minSDK = string.Empty,
                targetSDK = string.Empty,
                icon = string.Empty,
                name = string.Empty,
                per = string.Empty,
                package = string.Empty;
            
            
            foreach(string msg in model.Messages) {
                if(msg.StartsWith("application:")) {
                    icon = splitValueFromString(msg, @"icon='[\w-.\\\/]*[^']");
                    name = splitValueFromString(msg, @"label='[\w-.() \\\/\[\]]*[^']");
                    continue;
                }

                if (msg.StartsWith("package:")) {
                    ver = splitValueFromString(msg, @"versionName='[\w-.() \/]*[^']");
                    package = splitValueFromString(msg, @"name='[\w.]*[^']");
                    continue;
                }

                if (msg.StartsWith("sdkVersion:")) {
                    minSDK = splitValueFromString(msg, @"sdkVersion:'[0-9]{0,1}[^']");
                    continue;
                }

                if (msg.StartsWith("targetSdkVersion:")) {
                    targetSDK = splitValueFromString(msg, @"targetSdkVersion:'[0-9]{0,1}[^']");
                    continue;
                }

                if (msg.StartsWith("uses-permission:")) {
                    per = splitValueFromString(msg, @"'([A-Z0-9._])*");
                    permissions.Add(per);
                    continue;
                }

                if (msg.StartsWith("supports-screens:")) {
                    if (msg.Contains(ApkInfo.SmallScreen)) {
                        supportScrs.Add(ApkInfo.SmallScreen);
                    }
                    if (msg.Contains(ApkInfo.NormalScreen)) {
                        supportScrs.Add(ApkInfo.NormalScreen);
                    }
                    if (msg.Contains(ApkInfo.LargeScreen)) {
                        supportScrs.Add(ApkInfo.LargeScreen);
                    }
                    if (msg.Contains(ApkInfo.xLargeScreen)) {
                        supportScrs.Add(ApkInfo.xLargeScreen);
                    }
                    continue;
                }
            }
            
            return new ApkInfo() {
                FullPath = model.FilePath,
                AppName = name,
                PackageName = package,
                Version = ver,
                MinSDK = SDKInfo.GetInfo(minSDK),
                TargetSDK = SDKInfo.GetInfo(targetSDK),
                IconName = icon,
                Permissions = permissions,
                SupportScreens = supportScrs
            };
        }

        private static string splitValueFromString(string source, string pattern) {
            string temp = Regex.Match(source, pattern, RegexOptions.IgnoreCase).Value;
            return temp.Substring(temp.IndexOf("'") + 1);
        }
    }
}

