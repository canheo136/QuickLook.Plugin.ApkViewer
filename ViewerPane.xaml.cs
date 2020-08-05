using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using AAPTForNet;
using QuickLook.Common.Plugin;
using QuickLook.Common.ExtensionMethods;

namespace QuickLook.Plugin.ApkViewer {

    public partial class ViewerPane : UserControl {
        private ApkInfo _apkInfo = null;
        private ContextObject _quickLook = null;

        public ApkInfo ApkInfo {
            get => _apkInfo;
            set {
                _apkInfo = value;
                initGUI();
            }
        }

        public ViewerPane() {
            InitializeComponent();

            btnSwTheme.Source = new BitmapImage(new Uri(AAPTool.AppPath + @"\images\switch_theme.png"));
            btnSwTheme.MouseLeftButtonDown += (sender, e) => {
                bool isDark = _quickLook.Theme == Themes.Dark;

                // Make some effect with switch button.
                btnSwTheme.RenderTransform = new RotateTransform(
                    isDark ? 180 : 0,
                    btnSwTheme.Width / 2,
                    btnSwTheme.Height / 2
                );
                this.Resources["TextForeground"] = isDark ? Brushes.Black : Brushes.White;
                _quickLook.Theme = isDark ? Themes.Light : Themes.Dark;
            };
        }

        public ViewerPane(ContextObject ql) : this() {
            _quickLook = ql;
            
            // Highlight label on QuickLook default theme
            this.Resources["TextForeground"] = ql.Theme == Themes.Dark ?
                Brushes.White : Brushes.Black;
        }

        private void initGUI() {
            labelAppName.Content = ApkInfo.AppName;
            labelPckName.Content = ApkInfo.PackageName;
            labelVer.Content = ApkInfo.Version;
            labelMinSDK.Content = ApkInfo.MinSDK;
            labelTargetSDK.Content = ApkInfo.TargetSDK;
            labelSupportScr.Content = string.Join(", ", ApkInfo.SupportScreens);
            labelPckSize.Content = ApkInfo.PackageSize.ToPrettySize(2);

            if (ApkInfo.Permissions.Count != 0) {
                foreach (string per in ApkInfo.Permissions) {
                    permissionStack.Children.Add(new Label() {
                        // Disable access key (mnemonics)
                        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.label#remarks
                        Content = per.Replace("_", "__"),
                        Style = (Style)this.Resources["HoverableLabel"]
                    });
                }
            }
            else {
                panelPermission.Content = new Label() {
                    Content = "This package does not require any permissions",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Style = (Style)this.Resources["LabelBaseStyle"]
                };
            }

            string uri = ApkInfo.IconPath;

            if (ApkInfo.IconPath != string.Empty) {
                image.MouseLeftButtonDown += (sender, e) => {
                    System.Diagnostics.Process.Start("explorer.exe", ApkInfo.IconPath);
                };
            }
            else {
                image.ToolTip = "Default icon";
                uri = AAPTool.AppPath + @"\images\default_icon.png";
            }

            image.Source = new BitmapImage(new Uri(uri));
        }
    }
}
