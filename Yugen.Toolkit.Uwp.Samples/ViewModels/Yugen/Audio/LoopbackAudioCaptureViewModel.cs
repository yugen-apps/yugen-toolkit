//using CommunityToolkit.Mvvm.ComponentModel;
//using NAudio.Wave;
//using ScreenSenderComponent;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using Windows.Media.MediaProperties;
//using Windows.Storage;
//using Windows.Storage.Pickers;
//using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
//using Yugen.Toolkit.Uwp.Audio.Services.Common.Helpers;

//namespace Yugen.Audio.Samples.ViewModels
//{
//    public class LoopbackAudioCaptureViewModel : ObservableObject
//    {
//        private readonly List<byte> _bufferList = new List<byte>();
//        private AudioDevice _selectedAudioDevice;
//        private bool _isRecoding;
//        private LoopbackAudioCapture _loopbackAudioCapture;
//        private AudioEncodingProperties _audioEncodingProperties;
//        private string _playToggleButtonGlyph = "\uE7C8";

//        public ObservableCollection<AudioDevice> AudioDeviceCollection { get; set; } =
//            new ObservableCollection<AudioDevice>();

//        public AudioDevice SelectedAudioDevice
//        {
//            get => _selectedAudioDevice;
//            set => SetProperty(ref _selectedAudioDevice, value);
//        }

//        public bool IsRecording
//        {
//            get => _isRecoding;
//            set
//            {
//                if (_isRecoding != value)
//                {
//                    _isRecoding = value;

//                    if (_isRecoding)
//                    {
//                        StartRecording();
//                    }
//                    else
//                    {
//                        StopRecording();
//                    }

//                    PlayToggleButtonGlyph = _isRecoding ? "\uE71A" : "\uE7C8";
//                }
//            }
//        }

//        public string PlayToggleButtonGlyph
//        {
//            get => _playToggleButtonGlyph;
//            set => SetProperty(ref _playToggleButtonGlyph, value);
//        }

//        public async Task Initialize()
//        {
//            foreach (var deviceInfo in AudioDevicesHelper.DeviceInformationCollection)
//            {
//                AudioDeviceCollection.Add(new AudioDevice
//                {
//                    Driver = deviceInfo.Id,
//                    IsDefault = deviceInfo.IsDefault,
//                    Name = deviceInfo.Name
//                });
//            }

//            SelectedAudioDevice = AudioDeviceCollection.FirstOrDefault(x => x.Driver.Equals(AudioDevicesHelper.MasterAudioDeviceInformation.Id));

//            try
//            {
//                _loopbackAudioCapture = new LoopbackAudioCapture(SelectedAudioDevice.Driver);
//                await _loopbackAudioCapture.Start();
//                await _loopbackAudioCapture.Stop();
//                _loopbackAudioCapture = null;
//            }
//            catch (Exception)
//            {
//            }
//        }

//        private static async Task<StorageFile> SaveAs()
//        {
//            try
//            {
//                var picker = new FileSavePicker
//                {
//                    SuggestedFileName = "rec",
//                    DefaultFileExtension = ".wav"
//                };
//                picker.FileTypeChoices.Add(".wav", new List<string> { ".wav" });
//                return await picker.PickSaveFileAsync();
//            }
//            catch
//            {
//            }

//            return null;
//        }

//        private async void StartRecording()
//        {
//            _loopbackAudioCapture = new LoopbackAudioCapture(SelectedAudioDevice.Driver)
//            {
//                BufferReadyDelegate = LoopbackBufferReady
//            };
//            _bufferList.Clear();
//            await _loopbackAudioCapture.Start();
//        }

//        private async void StopRecording()
//        {
//            if (_loopbackAudioCapture != null &&
//                _loopbackAudioCapture.Started)
//            {
//                _audioEncodingProperties = _loopbackAudioCapture.EncodingProperties;
//                await _loopbackAudioCapture.Stop();
//                await SaveAudioFileAsync();
//                IsRecording = false;
//            }
//        }

//        private unsafe void LoopbackBufferReady(AudioClientBufferDetails details, out int numSamplesRead)
//        {
//            numSamplesRead = details.NumSamplesToRead;

//            byte* buffer = (byte*)details.DataPointer;
//            uint byteLength = (uint)details.ByteLength;

//            byte[] audioBuffer = new byte[byteLength];

//            Unsafe.CopyBlock(ref audioBuffer[0], ref *buffer, byteLength);

//            foreach (var b in audioBuffer)
//            {
//                _bufferList.Add(b);
//            }
//        }

//        private async Task SaveAudioFileAsync()
//        {
//            if (_audioEncodingProperties != null)
//            {
//                byte[] Audiobuffer = _bufferList.ToArray();
//                var audioFile = await SaveAs();

//                var s = new RawSourceWaveStream(new MemoryStream(Audiobuffer), WaveFormat.CreateIeeeFloatWaveFormat((int)_audioEncodingProperties.SampleRate, (int)_audioEncodingProperties.ChannelCount));
//                using (var writer = new WaveFileWriterRT(await audioFile.OpenStreamForWriteAsync(), s.WaveFormat))
//                {
//                    long outputLength = 0;
//                    var buffer = new byte[s.WaveFormat.AverageBytesPerSecond * 4];
//                    while (true)
//                    {
//                        int bytesRead = s.Read(buffer, 0, buffer.Length);
//                        if (bytesRead == 0)
//                        {
//                            // end of source provider
//                            break;
//                        }
//                        outputLength += bytesRead;
//                        // Write will throw exception if WAV file becomes too large
//                        writer.Write(buffer, 0, bytesRead);
//                    }
//                }
//            }
//        }

//        private string GetMessageForHResult(int hresult)
//        {
//            switch ((uint)hresult)
//            {
//                // MF_E_TRANSFORM_TYPE_NOT_SET
//                case 0xC00DA412:
//                    return "The combination of options you've chosen are not supported by your hardware.";

//                case 0x80070070:
//                    return "There is not enough space for recording in your device. ";

//                case 0xC00D4A44:
//                    return "The recorder wasn't able to capture enough frames";

//                default:
//                    return null;
//            }
//        }
//    }
//}