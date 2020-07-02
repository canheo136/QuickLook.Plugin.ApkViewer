using System.IO;

namespace AAPTForNet {
    public class ApkInfo {
        private readonly string unknownInfo = "<Empty>";
        private readonly string defaultIconName = "ic_launcher.png";

        private string _name;
        private string _package;
        private string _ver;
        private string _icon;

        public static readonly ApkInfo Empty = new ApkInfo();

        public string AppName => _name.Length > 0 ? _name : unknownInfo;
        public string PackageName => _package.Length > 0 ? _package : unknownInfo;
        public string Version => _ver.Length > 0 ? _ver : unknownInfo;
        public SDKInfo MinSDK { get; }
        public string IconName => _icon.Length > 4 ? _icon : defaultIconName;
        public string FullPath { get; }
        public long PackageSize => File.Exists(FullPath) ?
                    new FileInfo(FullPath).Length : 0;
        public string IconPath => (IconName != string.Empty && IconName != defaultIconName) ?
            AAPTool.TempPath + @"\" + IconName : string.Empty;
        public bool isEmpty { get; }

        internal ApkInfo() {
            isEmpty = true;
            MinSDK = SDKInfo.Unknown;
            _icon = FullPath = string.Empty;
            _ver = _package = _name = unknownInfo;
        }

        internal ApkInfo(string path, string name, string package, string ver, SDKInfo sdk, string iconName) {
            this.isEmpty = false;
            this.MinSDK = sdk;
            this.FullPath = path;
            this._name = name;
            this._package = package;
            this._ver = ver;
            this._icon = iconName.Length > 4 ? // Include name and extension
                Path.GetFileNameWithoutExtension(iconName) + ".png" : defaultIconName;
        }

        public override string ToString() {
            return string.Format("Name: {0};\nPackage: {1};\nVer: {2};\nMinSDK: {3};\nIcon: {4}\n",
                this.AppName, this.PackageName, this.Version, this.MinSDK.ToString(), this.IconPath);
        }
    }
}
