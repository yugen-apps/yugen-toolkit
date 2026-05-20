//using Microsoft.Extensions.DependencyInjection;
//using Windows.UI.Xaml.Controls;
//using Yugen.Audio.Samples.ViewModels;

//namespace Yugen.Toolkit.Uwp.Samples.Views.Yugen.Audio
//{
//    public sealed partial class LoopbackAudioCapturePage : Page
//    {
//        public LoopbackAudioCapturePage()
//        {
//            InitializeComponent();

//            DataContext = App.Current.Services.GetService<LoopbackAudioCaptureViewModel>();

//            this.Loaded += async (s, e) => await ViewModel.Initialize();
//        }

//        private LoopbackAudioCaptureViewModel ViewModel => (LoopbackAudioCaptureViewModel)DataContext;
//    }
//}
