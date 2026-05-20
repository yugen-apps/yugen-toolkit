using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Toolkit.Uwp.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private const string DarkThemeBackground = "#FF000000";
        private const string LightThemeBackground = "#FFFFFFFF";
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public ElementTheme Theme { get; set; } = ElementTheme.Default;

        public async Task InitializeAsync()
        {
            ElementTheme theme = LoadThemeFromSettingsAsync();
            await SetThemeAsync(theme);
        }

        public async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

            await SetRequestedThemeAsync();

            SaveThemeInSettingsAsync(Theme);
            
            //if (setTransparentTitleBar)
            //{
            //    SetTitleBarTheme(Theme);
            //}
        }

        public async Task SetRequestedThemeAsync()
        {
            foreach (CoreApplicationView view in CoreApplication.Views)
            {
                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = Theme;
                    }
                });
            }
        }

        //private void SetTitleBarTheme(ElementTheme theme)
        //{
        //    var buttonForegroundColor = GetThemeResource<Color>(theme, "TitleBarButtonForeground");
        //    var buttonHoverBackgroundColor = GetThemeResource<Color>(theme, "TitleBarButtonHoverBackground");
        //    var buttonHoverForegroundColor = GetThemeResource<Color>(theme, "TitleBarButtonHoverForeground");
        //    var buttonInactiveForegroundColor = GetThemeResource<Color>(theme, "TitleBarButtonForeground");

        //    TitleBarHelper.StyleTitleBar(buttonForegroundColor, buttonHoverBackgroundColor, buttonHoverForegroundColor, buttonInactiveForegroundColor);
        //}

        private T GetThemeResource<T>(ElementTheme theme, string resKey)
        {
            var isLightTheme = (theme == ElementTheme.Default)
                ? IsSystemThemeLight()
                : (theme == ElementTheme.Light);

            var themeKey = isLightTheme ? "Light" : "Dark";
            var themeDictionary = (ResourceDictionary)Application.Current.Resources.ThemeDictionaries[themeKey];
            return (T)themeDictionary[resKey];
        }

        private bool IsSystemThemeLight()
        {
            var defaultTheme = new UISettings();
            var uiTheme = defaultTheme.GetColorValue(UIColorType.Background).ToString();
            return uiTheme == LightThemeBackground;
        }

        private ElementTheme LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            var themeName = SettingsHelper.Read<string>(SettingsKey, HelperJsonSerializerContext.Default.String);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private void SaveThemeInSettingsAsync(ElementTheme theme) =>
            SettingsHelper.Write(SettingsKey, theme.ToString(), HelperJsonSerializerContext.Default.String);
    }
}
