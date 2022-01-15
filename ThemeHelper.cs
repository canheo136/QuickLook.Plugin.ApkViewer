using System;
using QuickLook.Common.Helpers;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.ApkViewer {
    internal class ThemeHelper {
        public static Themes TryGet() {
            var theme = SettingHelper.Get(
                id: nameof(ContextObject.Theme),
                failsafe: Themes.Dark.ToString(),
                typeof(ThemeHelper).Namespace);

            return Enum.TryParse(theme, out Themes output)
                ? output : Themes.Dark;
        }

        public static void Set(Themes theme) {
            SettingHelper.Set(
                id: nameof(ContextObject.Theme),
                theme.ToString(),
                typeof(ThemeHelper).Namespace);
        }
    }
}
