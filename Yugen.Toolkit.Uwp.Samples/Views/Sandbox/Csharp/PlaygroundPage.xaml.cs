using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Yugen.Toolkit.Uwp.Extensions;

namespace Yugen.Toolkit.Uwp.Samples.Views.Sandbox.Csharp
{
    public sealed partial class PlaygroundPage : Page
    {
        public PlaygroundPage()
        {
            this.InitializeComponent();
        }

        private async void OnLoadButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var file = await GetFile();

            byte[] bytes;
            if (file != null)
            {
                bytes = await file.ReadBytesAsync();
            }
            // bytes = null;
        }

        private async void OnLoadButton2Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var file = await GetFile();

            byte[] bytes;
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    using (var reader = new DataReader(stream.GetInputStreamAt(0)))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        bytes = new byte[stream.Size];
                        reader.ReadBytes(bytes);
                    }
                }
            }
            // bytes = null;
        }

        private async void OnLoadButton3Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var file = await GetFile();

            byte[] bytes;
            if (file != null)
            {

                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                }
            }
            // bytes = null;
        }

        private async Task<StorageFile> GetFile()
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
            };
            picker.FileTypeFilter.Add("*");

            return await picker.PickSingleFileAsync();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FlyoutButton.ContextFlyout.ShowAt((FrameworkElement)sender);
        }

        private void Image_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var flyout = FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
            var options = new FlyoutShowOptions()
            {
                // Position shows the flyout next to the pointer.
                // "Transient" ShowMode makes the flyout open in its collapsed state.
                Position = e.GetPosition((FrameworkElement)sender),
                ShowMode = FlyoutShowMode.Transient,
                Placement = FlyoutPlacementMode.TopEdgeAlignedLeft
            };
            flyout?.ShowAt((FrameworkElement)sender, options);
        }
    }

    //    public static class SettingsStorageExtensions
    //    {
    //        private const string FileExtension = ".json";

    //        public static bool IsRoamingStorageAvailable(this ApplicationData appData)
    //        {
    //            return appData.RoamingStorageQuota == 0;
    //        }

    //        public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
    //        {
    //            var file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.ReplaceExisting);
    //            var fileContent = await JsonProvider.StringifyAsync(content);

    //            await FileIO.WriteTextAsync(file, fileContent);
    //        }

    //        public static async Task<T> ReadAsync<T>(this StorageFolder folder, string name)
    //        {
    //            if (!File.Exists(Path.Combine(folder.Path, GetFileName(name))))
    //            {
    //                return default;
    //            }

    //            var file = await folder.GetFileAsync($"{name}.json");
    //            var fileContent = await FileIO.ReadTextAsync(file);

    //            return await JsonProvider.ToObjectAsync<T>(fileContent);
    //        }

    //        public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
    //        {
    //            if (content == null)
    //            {
    //                throw new ArgumentNullException(nameof(content));
    //            }

    //            if (string.IsNullOrEmpty(fileName))
    //            {
    //                throw new ArgumentException("ExceptionSettingsStorageExtensionsFileNameIsNullOrEmpty", nameof(fileName));
    //            }

    //            var storageFile = await folder.CreateFileAsync(fileName, options);
    //            await FileIO.WriteBytesAsync(storageFile, content);
    //            return storageFile;
    //        }

    //        public static async Task<byte[]> ReadFileAsync(this StorageFolder folder, string fileName)
    //        {
    //            var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);

    //            if (item != null && item.IsOfType(StorageItemTypes.File))
    //            {
    //                var storageFile = await folder.GetFileAsync(fileName);
    //                byte[] content = await storageFile.ReadBytesAsync();
    //                return content;
    //            }

    //            return null;
    //        }

    //        public static async Task<byte[]> ReadBytesAsync(this StorageFile file)
    //        {
    //            if (file != null)
    //            {
    //                using (IRandomAccessStream stream = await file.OpenReadAsync())
    //                {
    //                    using (var reader = new DataReader(stream.GetInputStreamAt(0)))
    //                    {
    //                        await reader.LoadAsync((uint)stream.Size);
    //                        var bytes = new byte[stream.Size];
    //                        reader.ReadBytes(bytes);
    //                        return bytes;
    //                    }
    //                }
    //            }

    //            return null;
    //        }

    //        private static string GetFileName(string name)
    //        {
    //            return string.Concat(name, FileExtension);
    //        }
    //    }

    //_ = System.Convert.ToDecimal("1.2345"); //new CultureInfo("en-US") //where the number separator is "."
    //        _ = System.Convert.ToDecimal("1.2345", new CultureInfo("it-IT")); //where the number separator is ","
    //        _ = System.Convert.ToDecimal("1.2345", CultureInfo.CurrentUICulture); //The above line returns my culture
    //        _ = System.Convert.ToDecimal("1.2345", CultureInfo.InvariantCulture); // http://stackoverflow.com/questions/9760237/what-does-cultureinfo-invariantculture-mean
    //        _ = System.Convert.ToDecimal("1,2345"); //new CultureInfo("en-US") //where the number separator is "."
    //        _ = System.Convert.ToDecimal("1,2345", new CultureInfo("it-IT")); //where the number separator is ","
    //        _ = System.Convert.ToDecimal("1,2345", CultureInfo.CurrentUICulture); //The above line returns my culture
    //        _ = System.Convert.ToDecimal("1,2345", CultureInfo.InvariantCulture); // http://stackoverflow.com/questions/9760237/what-does-cultureinfo-invariantculture-mean

    //    private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    //    {
    //        myProgressService = new MyProgressService(percent =>
    //        {
    //            MyProgressBar.Value = percent;
    //        });

    //        Increment();
    //    }

    //    private async void Increment()
    //    {
    //        Parallel.For(0, 50, async y =>
    //        {
    //            await Task.Delay(100);
    //            myProgressService.IncrementProgress();
    //        });

    //        await myProgressService.Increment(100);

    //        //Task.Run(async () =>
    //        //{
    //        //    while (percentage < 100)
    //        //    {
    //        //        await Task.Delay(100);
    //        //        IncrementProgress(progress);
    //        //    }
    //        //});

    //        //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
    //        //{
    //        //    await Increment(50);

    //        //    await Increment(100);
    //        //});
    //    }
    //}

    //public class MyProgressService
    //{
    //    private int percentage = 0;
    //    private readonly IProgress<int> _progress;

    //    public MyProgressService(Action<int> progress)
    //    {
    //        _progress = new Progress<int>(progress);
    //    }

    //    public async Task Increment(int total)
    //    {
    //        while (percentage < total)
    //        {
    //            await Task.Delay(100);
    //            IncrementProgress();
    //        }
    //    }

    //    public void IncrementProgress()
    //    {
    //        _progress.Report(++percentage);
    //    }
    //}

    //public static class FileHelper
    //{
    //    //http://windowsapptutorials.com/windows-10/application-data-in-universal-windows-apps/
    //    //access a file in the app package.
    //    //var uri = "ms-appx:///Assets/readium-js/dev/index_RequireJS_single-bundle.html"
    //    //var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

    //    //access a file in the app package
    //    //Package.Current.InstalledLocation = "C:\\Dev\\Kortext-UWP-Reader\\UWP\\samples\\ReadiumUwp\\ReadiumUWP\\bin\\x86\\Debug\\AppX\\"
    //    //var folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets\\readium-js\\dev\\");
    //    //var file = await folder.GetFileAsync("index_RequireJS_single-bundle.html");

    //    //access a file in the app package.
    //    //var file = await Package.Current.InstalledLocation.GetFileAsync("Assets\\readium-js\\dev\\index_RequireJS_single-bundle.html");

    //    //access a web file in the app package.
    //    //var uri = "ms-appx-web:///Assets/readium-js/dev/index_RequireJS_single-bundle.html"

    //    //access a file in local app data store
    //    //var uriFile = "ms-appdata:///local/file.ext";

    //    //access a file in local app data store 
    //    //ApplicationData.Current.LocalFolder = "C:\\Users\\emili\\AppData\\Local\\Packages\\Readium.UWP_q2jkdtkhjs7ty\\LocalState"
    //    //var folder = ApplicationData.Current.LocalFolder;
    //    //var file = await folder.GetFileAsync("index_RequireJS_single-bundle.html");

    //    public static async Task<string> OpenAsString(string path)
    //    {
    //        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
    //        return await StreamHelper.FileToString(file);
    //    }

    //    //public static async Task Write()
    //    //{
    //    //    var a = ApplicationData.Current.LocalCacheFolder;
    //    //    var s = ApplicationData.Current.LocalFolder;
    //    //    var d = ApplicationData.Current.LocalSettings;
    //    //    StorageFile sampleFile = await s.CreateFileAsync("sample.txt", CreationCollisionOption.ReplaceExisting);
    //    //    await FileIO.WriteTextAsync(sampleFile, "Swift as a shadow");

    //}
}