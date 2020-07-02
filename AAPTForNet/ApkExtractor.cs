using System;
using System.IO.Compression;

namespace AAPTForNet {
    /// <summary>
    /// Extract launcher icon from apk file.
    /// </summary>
    internal class ApkExtractor {

        protected ApkExtractor() {}
        
        public static ApkInfo ExtractIcon(DumpModel model, ApkInfo apk) {
            bool hasIcon = false;
            string iconName = string.Empty;

            using(var archive = ZipFile.OpenRead(model.FilePath)) {
                ZipArchiveEntry entry;
                int max = archive.Entries.Count - 1;
                
                // Loop from bottom of the collection.
                // The largest icon is usually position at the end of package
                // (sorting by alphabet)
                for(int i = max; i > 0; i--) {
                    entry = archive.Entries[i];

                    if(entry.Name.Equals(apk.IconName)) {
                        hasIcon = true;
                        iconName = DateTime.Now.ToString("yyyyMMddhhmmssffffff") + ".png";

                        entry.ExtractToFile(AAPTool.TempPath + @"\" + iconName);

                        break;
                    }
                }
            }

            return new ApkInfo(
                model.FilePath,
                apk.AppName, 
                apk.PackageName, 
                apk.Version, 
                apk.MinSDK, 
                hasIcon ? iconName : string.Empty
            );
        }
    }
}
