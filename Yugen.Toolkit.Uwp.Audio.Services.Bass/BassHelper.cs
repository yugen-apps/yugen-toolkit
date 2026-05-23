using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
	public static class BassHelper
    {
		public static async Task TryCopyDll()
        {
            StorageFolder appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var arc = DetectArchitecture();
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

        private static string DetectArchitecture()
        {
            try
            {
                // Get the architecture of the current package
                var arch = Windows.ApplicationModel.Package.Current.Id.Architecture;

                return arch switch
                {
                    ProcessorArchitecture.X86 => "x86",
                    ProcessorArchitecture.X64 => "x64",
                    ProcessorArchitecture.Arm => "ARM",
                    ProcessorArchitecture.Arm64 => "ARM64",
                    _ => null
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}