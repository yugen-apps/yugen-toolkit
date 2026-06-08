using System;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
	public static class BassHelper
    {
		public static async Task TryCopyDll()
        {
            StorageFolder appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var arc = SystemInfoHelper.Architecture;
            var bassFileNames = new[] { "bass.dll", "bass_fx.dll", "bassmix.dll" };

            foreach (var bassFileName in bassFileNames)
            {
                var bassItem = await appFolder.TryGetItemAsync(bassFileName);
                if (bassItem != null)
                {
                    continue;
                }

                bassItem = await appFolder.TryGetItemAsync($"Yugen.Toolkit.Uwp.Audio.Services.Bass\\runtimes\\{arc}\\{bassFileName}");
                if (bassItem is StorageFile bassFile)
                {
                    await bassFile.CopyAsync(appFolder, bassFileName);
                }
            }
        }
    }
}