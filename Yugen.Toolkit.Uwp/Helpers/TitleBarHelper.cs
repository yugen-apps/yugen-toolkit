using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Yugen.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// A <see langword="static"/> <see langword="class"/> with helper methods to manage the title bar
    /// </summary>
    public static class TitleBarHelper
    {
        /// <summary>
        /// Styles the title bar buttons according to the theme in use
        /// </summary>
        /// <param name="isThemeAware">Use them aware colors</param>
        public static void StyleTitleBar(bool isThemeAware = false)
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            // Active
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.Transparent;

            // Hover
            titleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.BlueViolet;

            //Inactive
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = Colors.Transparent;

            if (isThemeAware)
            {
                titleBar.ButtonForegroundColor = titleBar.ButtonHoverForegroundColor = titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(0x20, 0xFF, 0xFF, 0xFF);
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF);
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xC0, 0xFF, 0xFF, 0xFF);
                titleBar.InactiveForegroundColor = Color.FromArgb(0xA0, 0xA0, 0xA0, 0xA0);
            }
        }

        /// <summary>
        /// Sets up the app UI to be expanded into the title bar
        /// </summary>
        public static void ExpandViewIntoTitleBar()
        {
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        /// <summary>
        /// Makes a XAML element interact with the system as if it’s the title bar.
        /// </summary>
        /// <param name="value">
        /// Custom XAML content that should act as the title bar. To use multiple objects,
        ///  wrap them in a container element such as one derived from Panel.
        ///  </param>
        public static void SetTitleBar(UIElement value)
        {
            Window.Current.SetTitleBar(value);
        }

        // <summary>
        // Styles the title bar buttons according to the theme in use
        // </summary>
        //public static void StyleTitleBar(
        //    Color buttonForegroundColor, 
        //    Color buttonHoverBackgroundColor, 
        //    Color buttonHoverForegroundColor, 
        //    Color buttonInactiveForegroundColor)
        //{
        //    ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

        //    titleBar.ButtonForegroundColor = buttonForegroundColor;
        //    titleBar.ButtonHoverBackgroundColor = buttonHoverBackgroundColor;
        //    titleBar.ButtonHoverForegroundColor = buttonHoverForegroundColor;
        //    titleBar.ButtonInactiveForegroundColor = buttonInactiveForegroundColor;

        //}
    }
}
