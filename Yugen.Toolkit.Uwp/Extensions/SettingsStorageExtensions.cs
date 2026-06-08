using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Yugen.Toolkit.Uwp.Extensions
{
	public static class SettingsStorageExtensions
	{
		public static async Task<byte[]> ReadBytesAsync(this StorageFile file)
		{
			if (file != null)
			{
				using (IRandomAccessStream stream = await file.OpenReadAsync())
				{
					using (var reader = new DataReader(stream.GetInputStreamAt(0)))
					{
						await reader.LoadAsync((uint)stream.Size);
						var bytes = new byte[stream.Size];
						reader.ReadBytes(bytes);
						return bytes;
					}
				}
			}

			return null;
		}
	}
}