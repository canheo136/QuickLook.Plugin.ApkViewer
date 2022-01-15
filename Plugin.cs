using System;
using System.IO;
using System.Windows;
using AAPTForNet;
using QuickLook.Common.Helpers;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.ApkViewer {
    public class Plugin : IViewer {

        private string tempApk = string.Empty;

        public int Priority => 0;

        public void Init() { }

        public bool CanHandle(string path) => path.ToLower().EndsWith(".apk");

        public void Prepare(string path, ContextObject context) {
            context.Title = Path.GetFileName(path);
            context.TitlebarOverlap = false;
            context.FullWindowDragging = true;
            context.TitlebarBlurVisibility = false;
            context.TitlebarColourVisibility = false;
            context.PreferredSize = new Size { Width = 750, Height = 450 };

            tempApk = createTempApk(path);
        }

        public void View(string path, ContextObject context) {
            try {
                var apk = AAPTool.Decompile(tempApk);
                if (apk.IsEmpty)
                    context.ViewerContent = new ErrorContent();
                else
                    context.ViewerContent = new ViewerPane(context) { ApkInfo = apk };
            }
            catch (Exception e) {
                ProcessHelper.WriteLog($"{path}\r\n{e}");
                context.ViewerContent = new ErrorContent();
            }

            context.IsBusy = false;
        }

        public void Cleanup() {
            if (!tempApk.ToLower().EndsWith(".tmp"))
                return;

            try {
                File.Delete(tempApk);
            }
            catch (Exception e) {
                ProcessHelper.WriteLog(e.ToString());
            }
        }

        private string createTempApk(string sourceFile) {
            string tempFile = string.Empty;
            try {
                tempFile = Path.GetTempFileName();
            }
            catch (IOException) {
                tempFile = Path.Combine(Path.GetTempPath(), $"{nameof(ApkViewer)}.tmp");
            }

            try {
                File.Copy(sourceFile, tempFile, true);
                return tempFile;
            }
            catch {
                return sourceFile;
            }
        }

        private class ErrorContent : System.Windows.Controls.Label {
            public ErrorContent() {
                FontSize = 16;
                Content = "Can not load package.";
                Foreground = System.Windows.Media.Brushes.White;
                VerticalAlignment = VerticalAlignment.Center;
                HorizontalAlignment = HorizontalAlignment.Center;
            }
        }
    }
}
