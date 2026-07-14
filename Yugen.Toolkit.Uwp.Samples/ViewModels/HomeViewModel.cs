using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Yugen.Toolkit.Standard.Mvvm;

namespace Yugen.Toolkit.Uwp.Samples.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel()
        {
            DownloadTextCommand = new AsyncRelayCommand(DownloadTextAsync);
        }

        public string GitHubRepositoryUrl => "https://github.com/yugen-apps/yugen-toolkit";

        public IAsyncRelayCommand DownloadTextCommand { get; }

        private async Task<string> DownloadTextAsync()
        {
            await Task.Delay(3000); // Simulate a web request

            return "Hello world!";
        }
    }
}