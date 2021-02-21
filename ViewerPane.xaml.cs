using AAPTForNet.Models;

using QuickLook.Common.ExtensionMethods;
using QuickLook.Common.Plugin;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            btnSwTheme.MouseLeftButtonDown += (sender, e) => {
                _quickLook.Theme = _quickLook.Theme == Themes.Dark ?
                    Themes.Light : Themes.Dark;
            };
        }

        public ViewerPane(ContextObject ql) : this() {
            _quickLook = ql;
            _quickLook.PropertyChanged += (sender, e) => {
                if (e.PropertyName != nameof(_quickLook.Theme))
                    return;

                if (_quickLook.Theme == Themes.Dark) {
                    Resources["TextForeground"] = Brushes.White;
                    btnSwTheme.Source = Resources["LightSwImage"] as BitmapImage;
                }
                else {
                    Resources["TextForeground"] = Brushes.Black;
                    btnSwTheme.Source = Resources["DarkSwImage"] as BitmapImage;
                }
            };
            _quickLook.Theme = Themes.Dark; // Init default theme
        }

        private void initGUI() {
            tbAppName.Text      = ApkInfo.AppName;
            tbPckName.Text      = ApkInfo.PackageName;
            tbVerName.Text      = ApkInfo.VersionName;
            tbVerCode.Text      = ApkInfo.VersionCode;
            tbMinSDK.Text       = ApkInfo.MinSDK.ToString();
            tbTargetSDK.Text    = ApkInfo.TargetSDK.ToString();
            tbPckSize.Text      = ApkInfo.PackageSize.ToPrettySize(2);
            tbSupportScr.Text   = string.Join(", ", ApkInfo.SupportScreens);

            if (ApkInfo.Permissions.Count != 0) {
                var hoverableStyle = Resources["HoverableLabel"] as Style;

                foreach (string per in ApkInfo.Permissions) {
                    permissionStack.Children.Add(
                        new HoverableLabel(per, hoverableStyle, SelectableLabel_MouseDoubleClick)
                    );
                }
            }
            else {
                panelPermission.Content = new Label() {
                    Content = "This package does not require any permission",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Style = Resources["CommonStyle"] as Style
                };
            }

            if(ApkInfo.Icon.isImage) {
                var uri = new Uri(ApkInfo.Icon.RealPath);

                image.ToolTip = "Open image";
                image.Source = loadBitmapImage(uri);
                image.MouseLeftButtonDown += (sender, e) => {
                    Process.Start("explorer.exe", ApkInfo.Icon.RealPath);
                };
            }
            else {
                image.Source = Resources["DefaultIcon"] as BitmapImage;
            }
        }

        private BitmapImage loadBitmapImage(Uri source) {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            // Ignore previous image in cache
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            // Cached image, prevent file in use exception
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = source;
            bitmap.EndInit();
            return bitmap;
        }

        private void SelectableLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var ctrl = sender as TextBox;
            ctrl?.SelectAll();
        }

        private class HoverableLabel : TextBox {
            public HoverableLabel(string text, Style style, MouseButtonEventHandler dbClickHandler) {
                this.Text = text;
                this.Style = style;
                this.MouseDoubleClick += dbClickHandler;
            }
        }
    }
}
