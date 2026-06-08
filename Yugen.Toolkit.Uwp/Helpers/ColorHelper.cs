using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Yugen.Toolkit.Uwp.Helpers
{
    public static class ColorHelper
    {
        /// <summary>
        /// convert Windows.UI.Color to System.Drawing.Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Drawing.Color Convert(Windows.UI.Color color) => 
            System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        /// <summary>
        /// convert System.Drawing.Color to Windows.UI.Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Windows.UI.Color Convert(System.Drawing.Color color) => 
            Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);

        /// <summary>
        /// convert string hex to Windows.UI.Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Windows.UI.Color Convert(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte r = (byte)System.Convert.ToUInt32(hex.Substring(0, 2), 16);
            byte g = (byte)System.Convert.ToUInt32(hex.Substring(2, 2), 16);
            byte b = (byte)System.Convert.ToUInt32(hex.Substring(4, 2), 16);
            byte a = (byte)System.Convert.ToUInt32(hex.Substring(6, 2), 16);
            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// convert string hex to SolidColorBrush
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static SolidColorBrush ConvertHexToSolidColorBrush(string hex) => 
            new SolidColorBrush(Convert(hex));

        public static Color GradientColor(float mu)
        {
            var c = (byte)((Math.Sin(mu * Math.PI * 2) + 1) * 127.5);

            return Color.FromArgb(255, (byte)(255 - c), c, 220);
        }
    }
}