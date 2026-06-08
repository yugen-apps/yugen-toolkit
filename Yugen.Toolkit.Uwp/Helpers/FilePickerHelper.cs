using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Yugen.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Helper to open and save a file using the picker
    /// </summary>
    public static class FilePickerHelper
    {
		/// <summary>
		/// Open a file from the picker
		/// </summary>
		/// <param name="fileTypeExtensionFilter">Extension eg: .jpg</param>
		/// <param name="pickerLocationId"></param>
		/// <returns>StorageFile</returns>
		public static async Task<StorageFile> OpenFile(string fileTypeExtensionFilter, PickerLocationId pickerLocationId = PickerLocationId.Downloads) => 
            await OpenFile(new List<string> { fileTypeExtensionFilter }, pickerLocationId);

		/// <summary>
		/// Open a file from the picker
		/// </summary>
		/// <param name="fileTypeExtensionFilters">Extension List eg: .jpg, .png</param>
		/// <param name="pickerLocationId"></param>
		/// <returns>StorageFile</returns>
		public static async Task<StorageFile> OpenFile(IList<string> fileTypeExtensionFilters = null, PickerLocationId pickerLocationId = PickerLocationId.Downloads)
        {
            FileOpenPicker picker = GeneratePicker(fileTypeExtensionFilters, pickerLocationId);
            return await picker.PickSingleFileAsync();
        }

		/// <summary>
		/// Open files from the picker
		/// </summary>
		/// <param name="fileTypeExtensionFilter">Extension eg: .jpg</param>
		/// <param name="pickerLocationId"></param>
		/// <returns>StorageFile</returns>
		public static async Task<IReadOnlyList<StorageFile>> OpenFiles(string fileTypeExtensionFilter, PickerLocationId pickerLocationId = PickerLocationId.Downloads) => 
            await OpenFiles(new List<string> { fileTypeExtensionFilter }, pickerLocationId);

		/// <summary>
		/// Open files from the picker
		/// </summary>
		/// <param name="fileTypeExtensionFilters">Extension List eg: .jpg, .png</param>
		/// <param name="pickerLocationId"></param>
		/// <returns>StorageFile</returns>
		public static async Task<IReadOnlyList<StorageFile>> OpenFiles(IList<string> fileTypeExtensionFilters = null, PickerLocationId pickerLocationId = PickerLocationId.Downloads)
        {
            FileOpenPicker picker = GeneratePicker(fileTypeExtensionFilters, pickerLocationId);
            return await picker.PickMultipleFilesAsync();
        }

        private static FileOpenPicker GeneratePicker(IList<string> fileTypeExtensionFilters, PickerLocationId pickerLocationId)
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = pickerLocationId
            };

            if (fileTypeExtensionFilters == null)
            {
                picker.FileTypeFilter.Add("*");
            }
            else
            {
                foreach (var extension in fileTypeExtensionFilters)
                {
                    picker.FileTypeFilter.Add(extension);
                }
            }

            return picker;
        }


		/// <summary>
		/// Save a file with the picker
		/// </summary>
		/// <param name="suggestedFileName">Suggested file name</param>
		/// <param name="fileTypeName">File type name eg: Image</param>
		/// <param name="fileTypeExtension">Extension eg: .jpg</param>
		/// <param name="pickerLocationId"></param>
		/// <returns>StorageFile</returns>
		public static async Task<StorageFile> SaveFile(string suggestedFileName, string fileTypeName, string fileTypeExtension, PickerLocationId pickerLocationId = PickerLocationId.Downloads) =>
            await SaveFile(suggestedFileName, new Dictionary<string, List<string>>() { { fileTypeName, new List<string>() { fileTypeExtension } } }, pickerLocationId);

		/// <summary>
		/// Save a file with the picker
		/// </summary>
		/// <param name="suggestedFileName">Suggested file name</param>
		/// <param name="fileTypeChoices">File type name List eg: Image(.jpg,.png), Text(.txt)</param>
		/// <param name="pickerLocationId"></param>
		/// <returns>StorageFile</returns>
		public static async Task<StorageFile> SaveFile(string suggestedFileName, IDictionary<string, List<string>> fileTypeChoices, PickerLocationId pickerLocationId = PickerLocationId.Downloads)
        {
            FileSavePicker picker = new FileSavePicker
            {
                DefaultFileExtension = fileTypeChoices.First().Value.First(),
                SuggestedFileName = suggestedFileName,
                SuggestedStartLocation = pickerLocationId
            };

            foreach (var filter in fileTypeChoices)
            {
                picker.FileTypeChoices.Add(filter.Key, filter.Value);
            }

            return await picker.PickSaveFileAsync();
        }
    }
}