#pragma once
#include "AudioGraphHelper.g.h"
#include <wrl/client.h>

MIDL_INTERFACE("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")
IMemoryBufferByteAccess : ::IUnknown
{
	virtual HRESULT __stdcall GetBuffer(BYTE * *value, UINT32 * capacity) = 0;
};

namespace winrt::ScreenSenderComponent::implementation
{
	struct AudioGraphHelper : AudioGraphHelperT<AudioGraphHelper>
	{
		AudioGraphHelper() = default;

		Windows::Media::Audio::AudioFrameOutputNode FrameOutputNode() const { return m_frameOutputNode; }
		void FrameOutputNode(Windows::Media::Audio::AudioFrameOutputNode const& value) { m_frameOutputNode = value; }

		Windows::Media::Audio::AudioFrameInputNode FrameInputNode() const { return m_frameInputNode; }
		void FrameInputNode(Windows::Media::Audio::AudioFrameInputNode const& value) { m_frameInputNode = value; }

		Windows::Media::Audio::AudioGraph AudioGraph() const { return m_audioGraph; }
		void AudioGraph(Windows::Media::Audio::AudioGraph const& value) { m_audioGraph = value; }

		winrt::event_token InputFrameRequested(ScreenSenderComponent::InputFrameRequestedHandler const& handler) { return m_inputFrameRequested.add(handler); }
		void InputFrameRequested(winrt::event_token const& token) noexcept { m_inputFrameRequested.remove(token); }

		void LockFrameOutputNodeBuffer();
		void UnlockFrameOutputNodeBuffer();
		void RegisterInputFrameEvent();
		void UnregisterInputFrameEvent();
		int64_t CreateInputAudioFrame(uint32_t length);
		void EndInputAudioFrame();

	private:
		Windows::Media::Audio::AudioFrameOutputNode m_frameOutputNode{ nullptr };
		Windows::Media::Audio::AudioFrameInputNode m_frameInputNode{ nullptr };
		Windows::Media::Audio::AudioGraph m_audioGraph{ nullptr };

		winrt::event<ScreenSenderComponent::InputFrameRequestedHandler> m_inputFrameRequested;
		winrt::event_token m_inputFrameEvent{};

		Windows::Media::AudioFrame m_inputAudioFrame{ nullptr };
		Windows::Media::AudioBuffer m_inputFrameAudioBuffer{ nullptr };
		Windows::Foundation::IMemoryBufferReference m_inputFrameMemoryBufferReference{ nullptr };
		Windows::Media::AudioBuffer m_outputFrameAudioBuffer{ nullptr };
		Windows::Foundation::IMemoryBufferReference m_outputFrameMemoryBufferReference{ nullptr };
	};
}

namespace winrt::ScreenSenderComponent::factory_implementation
{
	struct AudioGraphHelper : AudioGraphHelperT<AudioGraphHelper, implementation::AudioGraphHelper>
	{
	};
}
