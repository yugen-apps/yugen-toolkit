using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.System;
using Windows.System.Profile;
using Windows.System.UserProfile;
using Yugen.Toolkit.Uwp.Extensions;

namespace Yugen.Toolkit.Uwp.Helpers
{
	public static class SystemInfoHelper
	{
		private static readonly ProcessorArchitecture _architecture;
		private static readonly CultureInfo _culture;
		private static readonly string _deviceFamily;
		private static readonly string _deviceManufacturer;
		private static readonly string _deviceModel;
		private static readonly bool _isFirstRun;
		private static readonly bool _isUpdated;
		private static readonly string _operatingSystem;
		private static readonly PackageVersion _operatingSystemVersion;
		private static readonly PackageVersion _packageVersion;

		static SystemInfoHelper()
		{
			var packageId = Package.Current.Id;
			var easClientDeviceInformation = new EasClientDeviceInformation();
			var versionInfo = AnalyticsInfo.VersionInfo;
			ulong version = ulong.Parse(versionInfo.DeviceFamilyVersion);

			_packageVersion = packageId.Version;

			_deviceManufacturer = easClientDeviceInformation.SystemManufacturer;
			_deviceModel = easClientDeviceInformation.SystemProductName;
			_deviceFamily = versionInfo.DeviceFamily;

			_operatingSystem = easClientDeviceInformation.OperatingSystem;
			_operatingSystemVersion = new PackageVersion
			{
				Major = (ushort)((version & 0xFFFF000000000000L) >> 48),
				Minor = (ushort)((version & 0x0000FFFF00000000L) >> 32),
				Build = (ushort)((version & 0x00000000FFFF0000L) >> 16),
				Revision = (ushort)(version & 0x000000000000FFFFL)
			};
			_architecture = packageId.Architecture;

			var localSettings = ApplicationData.Current.LocalSettings;
			_isFirstRun = DetectIfFirstRun(localSettings);
			_isUpdated = DetectIfAppUpdated(localSettings);

			try
			{
				var languages = GlobalizationPreferences.Languages;
				_culture = languages.Count > 0 ? new CultureInfo(languages[0]) : null;
			}
			catch
			{
				_culture = null;
			}
		}

		/// <summary>
		/// "2.0.59"
		/// </summary>
		public static string ApplicationVersion => _packageVersion.ToFormattedString(3);

		public static CultureInfo Culture => _culture;

		public static string DeviceFamily => _deviceFamily;

		/// <summary>
		/// "HP HP Spectre x360 Convertible 13-aw0xxx"
		/// </summary>
		public static string DeviceManufacturerAndModel => $"{_deviceManufacturer} {_deviceModel}";

		public static bool IsAppUpdated => _isUpdated;

		public static bool IsFirstRun => _isFirstRun;

		public static ProcessorArchitecture OperatingSystemArchitecture => _architecture;

		/// <summary>
		/// Example: "WINDOWS 10.0.18363.815"
		/// </summary>
		public static string OperatingSystemVersion => $"{_operatingSystem} {OSVersion}";

		/// <summary>
		/// Example: "10.0.18363.815"
		/// </summary>
		public static string OSVersion => _operatingSystemVersion.ToFormattedString();

		public static string SystemInfo => $"{OperatingSystemVersion} - {DeviceManufacturerAndModel}";

		private static bool DetectIfAppUpdated(ApplicationDataContainer settingsStorage)
		{
			var currentVersion = _packageVersion.ToFormattedString();

			if (settingsStorage.Values.TryGetValue(nameof(currentVersion), out var previousVersionObj))
			{
				var previousVersion = previousVersionObj as string;

				// There are two possible cases if the "currentVersion" key exists:
				//   1) The previous version is different than the current one. This means that the application
				//      has been updated since the last time this method was called. We can overwrite the saved
				//      setting for "currentVersion" to bring that value up to date.
				//   2) The previous version matches the current one: the app has just been reopened without updates.
				//      In this case we have nothing to do.
				if (currentVersion != previousVersion && previousVersion != null)
				{
					settingsStorage.Values[nameof(currentVersion)] = currentVersion;
					return true;
				}
			}
			else
			{
				// If the "currentVersion" key does not exist, it means that this is the first time this method
				// is ever called. That is, this is either the first time the app has been launched, or the first
				// time a previously existing app has run this method (or has run it after a new update of the app).
				// In this case, save the current version.
				settingsStorage.Values[nameof(currentVersion)] = currentVersion;
			}

			return false;
		}

		private static bool DetectIfFirstRun(ApplicationDataContainer settingsStorage)
		{
			if (settingsStorage.Values.ContainsKey(nameof(IsFirstRun)))
			{
				return false;
			}

			settingsStorage.Values[nameof(IsFirstRun)] = true;

			return true;
		}

		public static string Architecture
		{
			get
			{
				try
				{
					return _architecture switch
					{
						ProcessorArchitecture.X86 => "x86",
						ProcessorArchitecture.X64 => "x64",
						ProcessorArchitecture.Arm => "ARM",
						ProcessorArchitecture.Arm64 => "ARM64",
						_ => null
					};
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
