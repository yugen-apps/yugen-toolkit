#include "pch.h"
#include "LoopbackAudioCapture.h"
#include "LoopbackAudioCapture.g.cpp"

namespace winrt::ScreenSenderComponent::implementation
{
    LoopbackAudioCapture::LoopbackAudioCapture(hstring const& renderDevice)
        : m_device(renderDevice)
    {
    }

    Windows::Foundation::IAsyncAction LoopbackAudioCapture::Start()
    {
        if (m_started)
            throw winrt::hresult_error(E_FAIL, L"Cannot start loopback capture as it has already been started.");

        m_started = true;
        m_callback = new AudioClientCallback();

        IActivateAudioInterfaceCompletionHandler* handler = nullptr;
        winrt::check_hresult(m_callback->QueryInterface(__uuidof(IActivateAudioInterfaceCompletionHandler), reinterpret_cast<void**>(&handler)));

        IActivateAudioInterfaceAsyncOperation* operation = nullptr;
        auto hr = ActivateAudioInterfaceAsync(m_device.c_str(), __uuidof(IAudioClient3), nullptr, handler, &operation);
        // Extra releases: ActivateAudioInterfaceAsync adds two references but only releases one
        handler->Release();
        handler->Release();
        winrt::check_hresult(hr);

        // Wait for activation to complete
        co_await winrt::resume_on_signal(m_callback->completionEvent.get());

        m_client = m_callback->Client;
        if (m_client == nullptr)
        {
            m_started = false;
        }
        else
        {
            BeginCapture();
        }
        m_callback->Release();
        m_callback = nullptr;
    }

    Windows::Foundation::IAsyncAction LoopbackAudioCapture::Stop()
    {
        if (!m_started)
            throw winrt::hresult_error(E_FAIL, L"Cannot stop loopback capture as it has not yet started.");

        m_started = false;
        m_captureIndex++;

        if (m_capturing)
        {
            // Create stopping event and wait for capture thread to signal it
            m_stoppingEvent.attach(CreateEvent(nullptr, TRUE, FALSE, nullptr));
            co_await winrt::resume_on_signal(m_stoppingEvent.get());
        }

        if (m_client != nullptr)
        {
            m_client->Stop();
            m_client->Release();
            m_client = nullptr;
        }
        if (m_captureClient != nullptr)
        {
            m_captureClient->Release();
            m_captureClient = nullptr;
        }
    }

    void LoopbackAudioCapture::ChangeDevice(hstring const& renderDevice)
    {
        if (m_started)
            throw winrt::hresult_error(E_FAIL, L"Cannot change device while audio capture is running. Call Stop() first, change the device, then call Start().");
        m_device = renderDevice;
    }

    void LoopbackAudioCapture::BeginCapture()
    {
        WAVEFORMATEX* mixFormat = nullptr;
        m_client->GetMixFormat(&mixFormat);
        m_waveFormat = *mixFormat;

        if (m_suggestedEncodingProperties != nullptr)
        {
            if (m_suggestedEncodingProperties.Subtype() == L"Float")
            {
                m_waveFormat.wFormatTag = WAVE_FORMAT_IEEE_FLOAT;
            }
            else
            {
                throw winrt::hresult_error(E_FAIL, L"Format subtype must be float");
            }
            if (m_suggestedEncodingProperties.Bitrate() > 0)
                m_waveFormat.nAvgBytesPerSec = m_suggestedEncodingProperties.Bitrate() / 8;
            if (m_suggestedEncodingProperties.BitsPerSample() > 0)
                m_waveFormat.wBitsPerSample = static_cast<WORD>(m_suggestedEncodingProperties.BitsPerSample());
        }
        else
        {
            m_waveFormat.wFormatTag = WAVE_FORMAT_IEEE_FLOAT;
            m_waveFormat.wBitsPerSample = 32;
            m_waveFormat.nAvgBytesPerSec = (m_waveFormat.wBitsPerSample / 8) * m_waveFormat.nChannels * m_waveFormat.nSamplesPerSec;
            m_waveFormat.cbSize = 0;
        }

        winrt::check_hresult(m_client->Initialize(AUDCLNT_SHAREMODE_SHARED,
            AUDCLNT_STREAMFLAGS_LOOPBACK | AUDCLNT_STREAMFLAGS_EVENTCALLBACK,
            m_requestedBufferDuration, 0, &m_waveFormat, nullptr));

        UINT32 period = 0;
        winrt::check_hresult(m_client->GetCurrentSharedModeEnginePeriod(&mixFormat, &period));
        m_samplesPerQuantum = static_cast<int32_t>(period);

        m_actualWaveFormat = *mixFormat;
        m_encodingProperties = Windows::Media::MediaProperties::AudioEncodingProperties();
        m_encodingProperties.Bitrate(m_actualWaveFormat.nAvgBytesPerSec * 8);
        m_encodingProperties.ChannelCount(m_actualWaveFormat.nChannels);
        m_encodingProperties.BitsPerSample(m_actualWaveFormat.wBitsPerSample);
        m_encodingProperties.SampleRate(m_actualWaveFormat.nSamplesPerSec);
        switch (m_actualWaveFormat.wFormatTag)
        {
        case WAVE_FORMAT_IEEE_FLOAT:
            m_encodingProperties.Subtype(L"Float");
            break;
        case WAVE_FORMAT_PCM:
            m_encodingProperties.Subtype(L"PCM");
            break;
        }

        winrt::check_hresult(m_client->GetBufferSize(&m_bufferFrameSize));
        m_actualBufferDuration = static_cast<REFERENCE_TIME>(static_cast<double>(REFTIMES_PER_SEC) * m_bufferFrameSize / mixFormat->nSamplesPerSec);

        winrt::check_hresult(m_client->GetService(__uuidof(IAudioCaptureClient), reinterpret_cast<void**>(&m_captureClient)));

        m_eventHandle.attach(CreateEvent(nullptr, FALSE, FALSE, nullptr));
        winrt::check_hresult(m_client->SetEventHandle(m_eventHandle.get()));
        winrt::check_hresult(m_client->Start());

        m_captureIndex++;

        // Launch capture on a background thread
        auto weak = get_weak();
        auto captureIdx = m_captureIndex.load();
        Windows::System::Threading::ThreadPool::RunAsync([weak, captureIdx](Windows::Foundation::IAsyncAction const&)
        {
            if (auto strong = weak.get())
            {
                strong->CaptureThread();
            }
        });
    }

    void LoopbackAudioCapture::CaptureThread()
    {
        m_capturing = true;
        auto cIndex = m_captureIndex.load();
        UINT32 sampleCount = 0;
        DWORD flags = 0;
        BYTE* data = nullptr;

        ScreenSenderComponent::AudioClientBufferDetails args{};
        args.ChannelCount = m_actualWaveFormat.nChannels;
        args.BytesPerMonoSample = m_actualWaveFormat.wBitsPerSample / 8;
        args.BytesPerSample = args.BytesPerMonoSample * args.ChannelCount;

        while (m_capturing && m_started && cIndex == m_captureIndex.load())
        {
            WaitForSingleObject(m_eventHandle.get(), 2000);
            auto hr = m_captureClient->GetBuffer(&data, &sampleCount, &flags, nullptr, nullptr);
            if (hr == AUDCLNT_S_BUFFER_EMPTY || hr == AUDCLNT_E_OUT_OF_ORDER || hr == AUDCLNT_E_BUFFER_OPERATION_PENDING)
            {
                m_captureClient->ReleaseBuffer(sampleCount);
                data = nullptr;
                continue;
            }
            else if (hr != S_OK)
            {
                m_captureClient->ReleaseBuffer(sampleCount);
                data = nullptr;
                break;
            }

            if ((flags & AUDCLNT_BUFFERFLAGS_SILENT) != 0)
            {
                data = nullptr;
            }

            args.DataPointer = reinterpret_cast<int64_t>(data);
            args.NumSamplesToRead = sampleCount;
            args.ByteLength = static_cast<int64_t>(args.NumSamplesToRead) * args.BytesPerSample;
            int32_t readSamples = sampleCount;

            if (data != nullptr && m_bufferReadyDelegate != nullptr)
            {
                readSamples = m_bufferReadyDelegate(args);
            }
            m_captureClient->ReleaseBuffer(readSamples);
        }

        if (m_stoppingEvent)
        {
            SetEvent(m_stoppingEvent.get());
        }
        m_capturing = false;
    }
}
