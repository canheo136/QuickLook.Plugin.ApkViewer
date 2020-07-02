using System;
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
            btnSwTheme.MouseLeftButtonDown += switchTheme;
        }

        public ViewerPane(ContextObject ql) : this() {
            _quickLook = ql;
        }

        private void viewImage(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
            System.Diagnostics.Process.Start("explorer.exe", ApkInfo.IconPath);
        }

        private void switchTheme(object sender, MouseButtonEventArgs e) {
            e.Handled = true;
            // Make some effect with switch button.
            btnSwTheme.RenderTransform = new RotateTransform(
                _quickLook.Theme == Themes.Dark ? 180 : 0,
                btnSwTheme.Width / 2,
                btnSwTheme.Height / 2
            );

            _quickLook.Theme = (_quickLook.Theme == Themes.Dark) ? Themes.Light : Themes.Dark;
        }

        private void initGUI() {
            labelAppName.Content = ApkInfo.AppName;
            labelPckName.Content = ApkInfo.PackageName;
            labelVer.Content = ApkInfo.Version;
            labelSDK.Content = ApkInfo.MinSDK;
            labelSize.Content = ApkInfo.PackageSize.ToPrettySize(2);

            string uri = ApkInfo.IconPath;

            if(ApkInfo.IconPath != string.Empty) {
                image.MouseLeftButtonDown += viewImage;
            }
            else {
                image.ToolTip = "Default icon";
                uri = AAPTool.AppPath + @"\images\default_icon.png";
            }

            image.Source = new BitmapImage(new Uri(uri));
        }
    }
}
