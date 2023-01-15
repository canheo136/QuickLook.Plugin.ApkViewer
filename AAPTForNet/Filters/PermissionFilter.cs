using System.Collections.Generic;
using AAPTForNet.Models;

namespace AAPTForNet.Filters {
    internal class PermissionFilter : BaseFilter {
        private List<string> permissions = new List<string>();

        public override bool CanHandle(string msg) {
            return msg.StartsWith("uses-permission:");
        }

        public override void AddMessage(string msg) {
            // uses-permission: name='<per>'
            // -> ["uses-permission: name=", "<per, get this value!!!>", ""]
            permissions.Add(msg.Split(seperator)[1]);
        }

        public override ApkInfo GetAPK() {
            return new ApkInfo() {
                Permissions = permissions
            };
        }

        public override void Clear() {
            permissions.Clear();
        }
    }
}
