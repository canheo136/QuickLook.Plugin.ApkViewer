using AAPTForNet;

using QuickLook.Common.Helpers;
using QuickLook.Common.Plugin;

using System;
using System.IO;
using System.Windows;

namespace QuickLook.Plugin.ApkViewer {
    public class Plugin : IViewer {
        public int Priority => 0;

        public void Init() {}

        public bool CanHandle(string path) => path.ToLower().EndsWith(".apk");

        public void Prepare(string path, ContextObject context) {
            context.Title = Path.GetFileName(path);
            context.TitlebarOverlap = false;
            context.FullWindowDragging = true;
            context.TitlebarBlurVisibility = false;
            context.TitlebarColourVisibility = false;
            context.PreferredSize = new Size { Width = 750, Height = 450 };
        }

        public void View(string path, ContextObject context) {
            try {
                var apk = AAPTool.Decompile(path);
                if (apk.IsEmpty)
                    context.ViewerContent = new ErrorContent();
                else
                    context.ViewerContent = new ViewerPane(context) { ApkInfo = apk };
            }
            catch(Exception e) {
                ProcessHelper.WriteLog(e.ToString());
                context.ViewerContent = new ErrorContent();
            }
            
            context.IsBusy = false;
        }

        public void Cleanup() { }

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
