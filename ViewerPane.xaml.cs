using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AAPTForNet.Models;
using QuickLook.Common.Annotations;
using QuickLook.Common.ExtensionMethods;
using QuickLook.Common.Helpers;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.ApkViewer {

    public partial class ViewerPane : UserControl, INotifyPropertyChanged {
        private const string SETTING_THEME_ID = "Theme";

        private ApkInfo _apkInfo = null;
        private ContextObject _quickLook = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public ApkInfo ApkInfo {
            get => _apkInfo;
            set {
                _apkInfo = value;
                InitGUI();
            }
        }

        public Themes Theme {
            get => _quickLook?.Theme ?? Themes.Dark;
            set {
                _quickLook.Theme = value;
                OnPropertyChanged();
            }
        }

        public bool IsDark => Theme == Themes.Dark;

        public ViewerPane() {
            InitializeComponent();
            btnSwTheme.MouseLeftButtonDown += SwitchTheme;
        }

        public ViewerPane(ContextObject ql) : this() {
            _quickLook = ql;
            _quickLook.PropertyChanged += AfterThemeChanged;

            Theme = (Themes) SettingHelper.Get(SETTING_THEME_ID, 1, GetType().Namespace);
        }

        private void SwitchTheme(object sender, MouseButtonEventArgs e) {
            Theme = IsDark ? Themes.Light : Themes.Dark;
            SettingHelper.Set(SETTING_THEME_ID, (int) Theme, GetType().Namespace);
        }

        private void AfterThemeChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != nameof(Theme))
                return;

            var resourceUri = "/QuickLook.Common;component/Styles" +
                    $"/MainWindowStyles{(IsDark ? ".Dark" : "")}.xaml";

            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary {
                Source = new Uri(resourceUri, UriKind.Relative)
            });
        }

        private void InitGUI() {
            tbAppName.Text = ApkInfo.AppName;
            tbPckName.Text = ApkInfo.PackageName;
            tbVerName.Text = ApkInfo.VersionName;
            tbVerCode.Text = ApkInfo.VersionCode;
            tbMinSDK.Text = ApkInfo.MinSDK.ToString();
            tbTargetSDK.Text = ApkInfo.TargetSDK.ToString();
            tbPckSize.Text = ApkInfo.PackageSize.ToPrettySize(2);
            tbSupportScr.Text = string.Join(", ", ApkInfo.SupportScreens);

            if (ApkInfo.SupportedABIs.Count == 0) {
                labels.Children.Remove(lbAbis);
                textboxs.Children.Remove(tbAbis);
            }
            else {
                tbAbis.Text = string.Join(", ", ApkInfo.SupportedABIs);
            }

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

            if (ApkInfo.Icon.IsImage) {
                var uri = new Uri(ApkInfo.Icon.RealPath);

                image.ToolTip = "Open image";
                image.Source = LoadBitmapImage(uri);
                image.MouseLeftButtonDown += (sender, e) => {
                    Process.Start("explorer.exe", ApkInfo.Icon.RealPath);
                };
            }
            else {
                image.Source = Resources["DefaultIcon"] as BitmapImage;
            }
        }

        private BitmapImage LoadBitmapImage(Uri source) {
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectableLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var ctrl = sender as TextBox;
            ctrl?.SelectAll();
        }

        private class HoverableLabel : TextBox {
            public HoverableLabel(string text, Style style,
                MouseButtonEventHandler dbClickHandler) {
                Text = text;
                Style = style;
                MouseDoubleClick += dbClickHandler;
            }
        }
    }
}
