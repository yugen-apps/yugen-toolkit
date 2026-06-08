using Windows.UI.Xaml;

namespace Yugen.Toolkit.Uwp.Adapters
{
    /// <summary>
    /// Function you can use for x:bind instead of converter
    /// <code>
    /// <StackPanel xmlns:converters="using:Yugen.Toolkit.Uwp.Adapters"
    ///             Visibility="{x:Bind converters:DebugToVisibilityAdapter.Build()}">
    /// </StackPanel>
    /// </code>
    /// </summary>
    public static class DebugToVisibilityAdapter
    {
        /// <summary>
        /// Converts a DEBUG value to a Visibility value.
        /// </summary>
        /// <returns>
        /// Returns Visibility.Visible if true, else Visibility.Collapsed.
        /// </returns>
        public static Visibility Debug()
        {
#if DEBUG
            return Visibility.Visible;
#else
            return Visibility.Collapsed;
#endif
        }
    }
}