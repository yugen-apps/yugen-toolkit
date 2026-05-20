using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Services;
using Yugen.Audio.Samples.ViewModels;
using Yugen.Toolkit.Standard.Data.Extensions;
using Yugen.Toolkit.Standard.Data.Sample;
using Yugen.Toolkit.Standard.Data.Sample.Interfaces;
using Yugen.Toolkit.Standard.Data.Sample.Services;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Audio.Services.NAudio;
using Yugen.Toolkit.Uwp.Helpers;
using Yugen.Toolkit.Uwp.Samples.Constants;
using Yugen.Toolkit.Uwp.Samples.ViewModels;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Microsoft.Mvvm;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Sandbox.Csharp;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Sandbox.Mvvm;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Snippets.Converters;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Yugen.Collections;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Yugen.Controls;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Yugen.Data;
using Yugen.Toolkit.Uwp.Samples.ViewModels.Yugen.Helpers;
using Yugen.Toolkit.Uwp.Samples.Views;
using Yugen.Toolkit.Uwp.Samples.Views.Sandbox.Xaml;
using Yugen.Toolkit.Uwp.Services;
using Yugen.Toolkit.Uwp.Audio.Services.Common.Helpers;

namespace Yugen.Toolkit.Uwp.Samples
{
    /// <summary>
    /// Add a reference to Yugen.Toolkit.Standard.Data
    /// Add a reference to Yugen.Toolkit.Standard.Data.Sample
    /// 
    /// To Create a Migration you need a web or console .net core project
    /// 
    /// How To Create a Migration
    /// Select: Startup Project: 
    /// Yugen.Toolkit.Core.Sample or Yugen.Toolkit.Web.Sample
    /// Go To: Package Manager Console
    /// Select: Default Project: Yugen.Toolkit.Standard.Data.Sample
    /// (Optional) Write: Remove-Migration
    /// Write: Add-Migration {MigrationName}
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Register services
            Services = ConfigureServices();

            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        public new static App Current => (App)Application.Current;
		
		public IServiceProvider Services { get; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is AppShell shell))
            {
                await InitializeServices();

                // Initial UI styling
                TitleBarHelper.ExpandViewIntoTitleBar();
                TitleBarHelper.StyleTitleBar(true);

                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell { Language = ApplicationLanguages.Languages[0] };
                shell.MainFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: UWP Load state from previously suspended application
                }
            }

            // adds callbacks for Back requests and changes
            NavigationService.Initialize(typeof(App), shell.MainFrame, typeof(HomePage));

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (shell.MainFrame.Content == null)
            {
                // When the navigation stack isn't restored, navigate to the first page
                // suppressing the initial entrance animation.
                NavigationService.NavigateToPage(typeof(HomePage), e.Arguments, new SuppressNavigationTransitionInfo());
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private IServiceProvider ConfigureServices()
        {
            var logFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs\\Yugen.Toolkit.Log.");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .WriteTo.File(logFilePath, restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();

            //Log.Logger = new LoggerConfiguration()
            // .MinimumLevel.Debug()
            // .WriteTo.Debug()
            //  .WriteTo.Logger(l => l.Filter
            //      .ByIncludingOnly(e => e.Level == LogEventLevel.Information)
            //          .WriteTo.File($"{logFilePath}.Info", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information))
            //  .WriteTo.Logger(l => l.Filter
            //      .ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
            //          .WriteTo.File($"{logFilePath}.Warning", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information))
            //  .CreateLogger();

            Log.Debug("Serilog started Debug!");
            Log.Information("Serilog started Information!");
            Log.Warning("Serilog started Warning!");

            return new ServiceCollection()
                .AddDbContext<BloggingContext>(options => options.UseSqlite($"Data Source={DataConstants.DbFileName}"))
                .AddUnitOfWork<BloggingContext>()
                // Data
                // Use Base Repository
                //.AddTransient<IBlogRepository, BlogRepository>()
                //.AddSingleton<IBlogRepositoryService, BlogRepositoryService>()\
                // Use Base Service
                .AddSingleton<IBlogService, BlogService>()
                // Services
                .AddSingleton<ITestService, TestService>()
                .AddSingleton<IThemeSelectorService, ThemeSelectorService>()
                .AddSingleton<IAudioGraphAudioPlayer, AudioGraphAudioPlayer>()
                .AddTransient<IWaveformService, WaveformService>()
                .AddTransient<IBPMService, BPMService>()
                // Root
                .AddSingleton<AppShellViewModel>()
                .AddTransient<HomeViewModel>()
                .AddTransient<SettingsViewModel>()
                // Microsoft
                .AddTransient<CommandViewModel>()
                .AddTransient<MediatorViewModel>()
                .AddTransient<NavigationParameterViewModel>()
                .AddTransient<NavigationViewModel>()
                .AddTransient<ObservableObjectViewModel>()
                .AddTransient<XamlUICommandViewModel>()
                // Sandbox
                .AddTransient<DeferralViewModel>()
                .AddTransient<ImagesGridViewModel>()
                // Snippets
                .AddTransient<EnumToBooleanConverterViewModel>()
                // Yugen
                .AddTransient<CollectionViewModel>()
                .AddTransient<GroupedCollectionViewModel>()
                .AddTransient<GraphViewModel>()
                //.AddTransient<SampleInAppControlViewModel>()
                .AddTransient<ValidationViewModel>()
                .AddTransient<YugenDialogViewModel>()
                .AddTransient<DataViewModel>()
                .AddTransient<FindControlViewModel>()
                .AddTransient<ObservableSettingsViewModel>()
                .AddSingleton<AudioFrameInputNodeViewModel>()
                //.AddSingleton<AudioGraphViewModel>()
                .AddSingleton<BassViewModel>()
                .AddSingleton<CsCoreViewModel>()
                .AddSingleton<SharpDXViewModel>()
                //.AddSingleton<VuBarsVieModel>()
                .AddSingleton<DeckViewModel>()
                .AddSingleton<VinylViewModel>()
                .AddSingleton<WaveformViewModel>()
                //.AddSingleton<LoopbackAudioCaptureViewModel>()
                // Log
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSerilog(dispose: true);
                })
                .BuildServiceProvider();

                // services.AddTransient<IRestClientService, RestClientService>(s => new RestClientService(CacheProvider.Content.Settings.Default.ApiUrl));
                // (s => new SyncAttributeTypeService(s.GetService<IAttributeTypeService>()));
        }

        private async Task InitializeServices()
        {
            var bloggingContext = Services.GetService<BloggingContext>();
            //var isCreated = bloggingContext.Database.EnsureCreated();
            //bloggingContext.Database.Migrate();

            await Services.GetService<IThemeSelectorService>().InitializeAsync();
            await AudioDevicesHelper.Initialize();
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Frame rootFrame = new Frame();
            Window.Current.Content = rootFrame;

            this.UnhandledException -= OnUnhandledException;

            rootFrame.Navigate(typeof(RsodPage), e);
        }
    }
}

// https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/dependency-injection
// https://stackoverflow.com/questions/39174989/how-to-register-multiple-implementations-of-the-same-interface-in-asp-net-core
// https://stackoverflow.com/questions/42221895/how-to-get-an-instance-of-iserviceprovider-in-net-core