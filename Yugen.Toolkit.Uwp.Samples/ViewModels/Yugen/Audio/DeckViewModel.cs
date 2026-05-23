using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.System;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Audio.Services.Bass;
using Yugen.Toolkit.Uwp.Extensions;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class DeckViewModel : ViewModelBase
	{
		private IAudioPlaybackService _audioPlayer = new AudioPlaybackService(new AudioDeviceService());

		private readonly IWaveformService _waveformService;
        private IBPMService _bpmService;
        private double _bpm;
        private List<(float min, float max)> _peakList;

        public DeckViewModel(
            IBPMService bpmService,
            IWaveformService waveformRendererService)
        {
            _bpmService = bpmService;
            _waveformService = waveformRendererService;

            _audioPlayer.Initialize();

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            PlayCommand = new RelayCommand(PlayCommandBehavior);
        }

        public ICommand OpenCommand { get; }

        public ICommand PlayCommand { get; }

        public double Bpm
        {
            get => _bpm;
            set => SetProperty(ref _bpm, value);
        }

        public List<(float min, float max)> PeakList
        {
            get => _peakList;
            set => SetProperty(ref _peakList, value);
        }

        public async Task GenerateAudioData(Stream stream)
        {
            List<(float min, float max)> peakList = null;

            await Task.Run(() =>
            {
                peakList = _waveformService.GenerateAudioData(stream);
            });

            PeakList = peakList;
        }

        private async Task OpenCommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                     new List<string> { ".mp3" },
                     PickerLocationId.MusicLibrary
                 );

            if (audioFile != null)
            {
                var bytes = await audioFile.ReadBytesAsync();
                await _audioPlayer.LoadSong(bytes);

                var stream = await audioFile.OpenStreamForReadAsync();

                //MemoryStream fileStream = new MemoryStream();
                //await stream.CopyToAsync(fileStream);
                //await _audioPlayer.LoadStream(fileStream);
                //stream.Position = 0;

                MemoryStream waveformStream = new MemoryStream();
                await stream.CopyToAsync(waveformStream);
                await GenerateAudioData(waveformStream);
                stream.Position = 0;

                MemoryStream bpmStream = new MemoryStream();
                await stream.CopyToAsync(bpmStream);
                var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                await Task.Run(() =>
                {
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        Bpm = _bpmService.Decoding(bpmStream);
                    });
                });
            }
        }

        private void PlayCommandBehavior()
        {
            _audioPlayer.TogglePlay(false);
        }
    }
}