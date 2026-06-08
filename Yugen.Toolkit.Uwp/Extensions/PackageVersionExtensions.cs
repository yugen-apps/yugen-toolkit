using System;
using Windows.ApplicationModel;

namespace Yugen.Toolkit.Uwp.Extensions
{
	/// <summary>
	/// This class provides static extension methods for <see cref="PackageVersion"/>.
	/// </summary>
	public static class PackageVersionExtensions
	{
		/// <summary>
		/// Returns a string representation of a version with the format 'Major.Minor.Build.Revision'.
		/// </summary>
		/// <param name="packageVersion">The <see cref="PackageVersion"/> to convert to a string</param>
		/// <param name="significance">The number of version numbers to return, default is 4 for the full version number.</param>
		/// <returns>Version string of the format 'Major.Minor.Build.Revision'</returns>
		/// <example>
		/// Package.Current.Id.Version.ToFormattedString(2); // Returns "7.0" for instance.
		/// </example>
		public static string ToFormattedString(this PackageVersion packageVersion, int significance = 4)
		{
			switch (significance)
			{
				case 4:
					return $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";

				case 3:
					return $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}";

				case 2:
					return $"{packageVersion.Major}.{packageVersion.Minor}";

				case 1:
					return $"{packageVersion.Major}";
			}

			static string ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException(nameof(significance), "Value must be a value 1 through 4.");

			return ThrowArgumentOutOfRangeException();
		}
	}
}
