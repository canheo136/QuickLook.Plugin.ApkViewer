using System.Collections.Generic;
using System.IO;

namespace AAPTForNet {
    /// <summary>
    /// Android Assert Packing Tool for NET
    /// </summary>
    public class AAPTool : System.Diagnostics.Process {
        private static readonly string _tempFolderPath = Path.GetTempPath() + @"QL.ApkViewer";
        
        public static string AppPath => Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
        public static string TempPath {
            get {
                if(!Directory.Exists(_tempFolderPath)) {
                    Directory.CreateDirectory(_tempFolderPath);
                }
                return _tempFolderPath;
            }
        }

        protected AAPTool() {
            this.StartInfo.FileName = AppPath + @"\tool\aapt.exe"; // The name of the executable file
            this.StartInfo.CreateNoWindow = true;
            this.StartInfo.UseShellExecute = false; // For read output data
            this.StartInfo.RedirectStandardError = true;
            this.StartInfo.RedirectStandardOutput = true;
            this.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding("utf-8");
        }

        protected new bool Start(string args) {
            this.StartInfo.Arguments = args;
            return base.Start();
        }

        private static DumpModel dump(string path) {
            if (!File.Exists(path))
                throw new IOException("File not found.");
            
            var output = new List<string>();
            var aapt = new AAPTool();
                aapt.Start(string.Format("dump badging \"{0}\"", path));

            while(!aapt.StandardOutput.EndOfStream) {
                // Read output line by line, more convenient for parse (than ReadToEnd)
                output.Add(aapt.StandardOutput.ReadLine());
            }

            while(!aapt.StandardError.EndOfStream) {
                output.Add(aapt.StandardError.ReadLine());
            }

            aapt.WaitForExit();
            aapt.Close();

            // An error have only 2 messages
            return new DumpModel(path, output.Count > 5, output);
        }

        public static ApkInfo Decompile(string path) {
            var dummModel = dump(path);

            if(dummModel.isSuccess) {
                var apk = ApkParser.Parse(dummModel);
                return ApkExtractor.ExtractIcon(dummModel, apk);
            }

            return ApkInfo.Empty;
        }

        public static void Init() {
            // Clear temp folder.
            // Should be called when QuickLook starts
            if(Directory.Exists(_tempFolderPath))
                Directory.Delete(_tempFolderPath, true);
        }
    }
}
