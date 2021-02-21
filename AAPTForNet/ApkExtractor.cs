using AAPTForNet.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace AAPTForNet {
    internal class ApkExtractor {

        public static DumpModel ExtractManifest(string path) {
            return AAPTool.dumpManifest(path);
        }

        /// <summary>
        /// Find the icon with maximum config (largest), then extract to file
        /// </summary>
        /// <param name="path"></param>
        public static Icon ExtractLargestIcon(string path) {
            var iconTable = ExtractIconTable(path);
            var largestIcon = ExtractLargestIcon(iconTable);
                largestIcon.RealPath = ExtractIconImage(path, largestIcon);
            return largestIcon;
        }

        private static Dictionary<string, Icon> ExtractIconTable(string path) {
            var iconID = ExtractIconID(path);
            return ExtractIconTable(path, iconID);
        }

        /// <summary>
        /// Extract resource id of launch icon from manifest tree
        /// </summary>
        /// <param name="path"></param>
        /// <returns>icon id</returns>
        private static string ExtractIconID(string path) {
            int iconIndex = 0;
            var manifestTree = AAPTool.dumpManifestTree(
                path,
                (m, i) => {
                    if (m.Contains("android:icon")) {
                        iconIndex = i;
                        return true;
                    }
                    return false;
                }
            );

            if (iconIndex == 0) // Package without launcher icon
                return string.Empty;

            if (manifestTree.isSuccess) {
                string msg = manifestTree.Messages[iconIndex];
                return msg.Split('@')[1];
            }

            return string.Empty;
        }

        private static Dictionary<string, Icon> ExtractIconTable(string path, string iconID) {
            if (string.IsNullOrEmpty(iconID))
                return new Dictionary<string, Icon>();

            var matchedEntry = false;
            var indexes = new List<int>();  // Get position of icon in resource list
            var resTable = AAPTool.dumpResources(path, (m, i) => {
                // Dump resources and get icons,
                // terminate when meet the end of mipmap entry,
                // icons are in 'drawable' or 'mipmap' resource
                if (m.Contains(iconID) && !m.Contains("flags"))
                    indexes.Add(i);

                if (!matchedEntry) {
                    if (m.Contains("mipmap/"))
                        matchedEntry = true;    // Begin mipmap entry
                }
                else {
                    if (m.Contains("entry")) {  // Next entry, terminate
                        matchedEntry = false;
                        return true;
                    }
                }
                return false;
            });

            return createIconTable(indexes, resTable.Messages);
        }

        // Create table like below
        //  configs  |    mdpi           hdpi    ...    anydpi
        //  icon     |    icon1          icon2   ...    icon4
        private static Dictionary<string, Icon> createIconTable(List<int> positions, List<string> messages) {
            if (positions.Count == 0 || messages.Count <= 2)    // If dump failed
                return new Dictionary<string, Icon>();

            const char seperator = '\"';
            // Prevent duplicate key when add to Dictionary,
            // because comparison statement with 'hdpi' in config's values,
            // reverse list and get first elem with LINQ
            var configNames = Enum.GetNames(typeof(Configs)).Reverse();
            var iconTable = new Dictionary<string, Icon>();
            string msg, iconName, config;

            foreach (int index in positions) {
                for (int i = index; ; i--) {
                    // Go prev to find config
                    msg = messages[i];

                    if (msg.Contains("entry"))  // Out of entry and not found
                        break;
                    if (msg.Contains("config") && configNames.Any(c => msg.Contains(c))) {
                        // Match with predefined configs,
                        // go next to get icon name
                        iconName = messages[index + 1]
                            .Split(seperator)
                            .FirstOrDefault(n => n.Contains("/"));
                        config = configNames.FirstOrDefault(c => msg.Contains(c));

                        if (!iconTable.ContainsKey(config))
                            iconTable.Add(config, new Icon(iconName));

                        break;
                    }
                }
            }
            return iconTable;
        }

        private static string ExtractIconImage(string path, Icon icon) {
            if (Icon.Default.Equals(icon))
                return "markup.xml";    // To use default icon

            string tempPath = Path.Combine(Path.GetTempPath(), "AAPToolTempImage.png");
            TryExtractIconImage(path, icon.IconName, tempPath);
            return tempPath;
        }

        private static void TryExtractIconImage(string path, string iconName, string desFile) {
            try {
                ExtractIconImage(path, iconName, desFile);
            }
            catch(ArgumentException) {}
        }

        /// <summary>
        /// Extract icon with name @iconName from @path to @desFile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="iconName"></param>
        /// <param name="desFile"></param>
        private static void ExtractIconImage(string path, string iconName, string desFile) {
            if (iconName.EndsWith(".xml") || !File.Exists(path))
                throw new ArgumentException("Invalid params");

            using (var archive = ZipFile.OpenRead(path)) {
                ZipArchiveEntry entry;

                for (int i = archive.Entries.Count - 1; i > 0; i--) {
                    entry = archive.Entries[i];

                    if (entry.FullName.Equals(iconName)) {
                        entry.ExtractToFile(desFile, true);
                        break;
                    }
                }
            }
        }

        private static Icon ExtractLargestIcon(Dictionary<string, Icon> iconTable) {
            if (iconTable.Count == 0)
                return Icon.Default;

            var icon = Icon.Default;
            var configNames = Enum.GetNames(typeof(Configs)).ToList();
                configNames.Sort(new ConfigComparer());

            foreach (string cfg in configNames) {
                // Get the largest icon image, skip markup file (xml)
                if (iconTable.TryGetValue(cfg, out icon)) {
                    if (icon.IconName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        continue;
                    break;  // Largest icon here :)
                }
            }

            return icon ?? Icon.Default;
        }
        
        /// <summary>
        /// DPI config comparer, ordered by desc (largest first)
        /// </summary>
        private class ConfigComparer : IComparer<string> {
            public int Compare(string x, string y) {
                Enum.TryParse<Configs>(x, out Configs ex);
                Enum.TryParse<Configs>(y, out Configs ey);
                return ex > ey ? -1 : 1;
            }
        }
    }
}
