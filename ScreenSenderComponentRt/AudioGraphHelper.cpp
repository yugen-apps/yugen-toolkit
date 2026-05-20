#include "pch.h"
#include "AudioGraphHelper.h"
#include "AudioGraphHelper.g.cpp"

namespace winrt::ScreenSenderComponent::implementation
{
    void AudioGraphHelper::LockFrameOutputNodeBuffer()
    {
        auto frame = m_frameOutputNode.GetFrame();
        m_outputFrameAudioBuffer = frame.LockBuffer(Windows::Media::AudioBufferAccessMode::Read);
        auto dataLength = m_outputFrameAudioBuffer.Length();
        m_outputFrameMemoryBufferReference = m_outputFrameAudioBuffer.CreateReference();

        auto interop = m_outputFrameMemoryBufferReference.as<IMemoryBufferByteAccess>();
        uint8_t* location = nullptr;
        uint32_t capacity = 0;
        winrt::check_hresult(interop->GetBuffer(&location, &capacity));

        auto DataLocation = reinterpret_cast<int64_t>(location);
        auto DataLength = dataLength;
    }

    void AudioGraphHelper::UnlockFrameOutputNodeBuffer()
    {
        m_outputFrameMemoryBufferReference.Close();
        m_outputFrameMemoryBufferReference = nullptr;
        m_outputFrameAudioBuffer.Close();
        m_outputFrameAudioBuffer = nullptr;
    }

    void AudioGraphHelper::RegisterInputFrameEvent()
    {
        m_inputFrameEvent = m_frameInputNode.QuantumStarted([this](Windows::Media::Audio::AudioFrameInputNode const&, Windows::Media::Audio::FrameInputNodeQuantumStartedEventArgs const& args)
        {
            m_inputFrameRequested(args.RequiredSamples());
        });
    }

    void AudioGraphHelper::UnregisterInputFrameEvent()
    {
        m_frameInputNode.QuantumStarted(m_inputFrameEvent);
    }

    int64_t AudioGraphHelper::CreateInputAudioFrame(uint32_t length)
    {
        m_inputAudioFrame = Windows::Media::AudioFrame(length);
        m_inputFrameAudioBuffer = m_inputAudioFrame.LockBuffer(Windows::Media::AudioBufferAccessMode::Read);
        m_inputFrameMemoryBufferReference = m_inputFrameAudioBuffer.CreateReference();

        auto interop = m_inputFrameMemoryBufferReference.as<IMemoryBufferByteAccess>();
        uint8_t* location = nullptr;
        uint32_t capacity = 0;
        winrt::check_hresult(interop->GetBuffer(&location, &capacity));

        return reinterpret_cast<int64_t>(location);
    }

    void AudioGraphHelper::EndInputAudioFrame()
    {
        m_inputFrameMemoryBufferReference.Close();
        m_inputFrameMemoryBufferReference = nullptr;
        m_inputFrameAudioBuffer.Close();
        m_inputFrameAudioBuffer = nullptr;
        m_frameInputNode.AddFrame(m_inputAudioFrame);
    }
}
