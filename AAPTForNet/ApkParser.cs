using System.Collections.Generic;
using System.Linq;
using AAPTForNet.Filters;
using AAPTForNet.Models;

namespace AAPTForNet {
    /// <summary>
    /// Parse output messages from AAPTool
    /// </summary>
    internal class ApkParser {
        public static ApkInfo Parse(DumpModel model) {
            if (!model.IsSuccess)
                return new ApkInfo();

            var filters = new List<BaseFilter>() {
                new ABIFilter(),
                new SDKFilter(),
                new PackageFilter(),
                new PermissionFilter(),
                new SupportScrFilter(),
                new ApplicationFilter()
            };

            foreach (string msg in model.Messages) {
                foreach (var f in filters) {
                    if (f.CanHandle(msg)) {
                        f.AddMessage(msg);
                        break;
                    }
                }
            }

            return ApkInfo.Merge(filters.Select(f => f.GetAPK()));
        }
    }
}

