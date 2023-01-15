using System;
using System.Linq;
using AAPTForNet.Models;

namespace AAPTForNet.Filters {
    /// <summary>
    /// Application Binary Interface Filter
    /// </summary>
    /// <remarks>https://developer.android.com/ndk/guides/abis</remarks>
    internal class ABIFilter : BaseFilter {

        private string[] segments = new string[] { };

        public override bool CanHandle(string msg)
            => msg.StartsWith("native-code:");

        public override void AddMessage(string msg) {
            segments = msg.Split(new char[2] { ' ', '\'' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override ApkInfo GetAPK() {
            return new ApkInfo() {
                SupportedABIs = segments.Skip(1).ToList()   // Skip "native-code"
            };
        }

        public override void Clear() => throw new NotImplementedException();
    }
}
