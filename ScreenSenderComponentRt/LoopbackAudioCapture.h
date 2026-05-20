#pragma once
#include "LoopbackAudioCapture.g.h"
#include <winrt/Windows.System.Threading.h>
#include <Audioclient.h>
#include <mmdeviceapi.h>

#define REFTIMES_PER_SEC  10000000
#define REFTIMES_PER_MILLISEC  10000

namespace winrt::ScreenSenderComponent::implementation
{
	// Internal COM class – not projected via WinRT
	class AudioClientCallback : public IActivateAudioInterfaceCompletionHandler, public IUnknown, public IAgileObject
	{
	public:
		winrt::handle completionEvent{ CreateEvent(nullptr, TRUE, FALSE, nullptr) };
		IAudioClient3* Client = nullptr;

		HRESULT STDMETHODCALLTYPE ActivateCompleted(IActivateAudioInterfaceAsyncOperation* operation) override
		{
			HRESULT resultCode = 0;
			IUnknown* activatedInterface = nullptr;
			auto hr = operation->GetActivateResult(&resultCode, &activatedInterface);
			if (hr == S_OK && resultCode == S_OK && activatedInterface)
			{
				activatedInterface->QueryInterface<IAudioClient3>(&Client);
				activatedInterface->Release();
			}
			SetEvent(completionEvent.get());
			return hr;
		}

		ULONG STDMETHODCALLTYPE AddRef() override { return ++m_refCount; }
		ULONG STDMETHODCALLTYPE Release() override
		{
			auto count = --m_refCount;
			if (count == 0) delete this;
			return count;
		}
		HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppvObject) override
		{
			if (riid == IID_IUnknown)
			{
				AddRef();
				*ppvObject = static_cast<IUnknown*>(this);
				return S_OK;
			}
			else if (riid == __uuidof(IActivateAudioInterfaceCompletionHandler))
			{
				AddRef();
				*ppvObject = static_cast<IActivateAudioInterfaceCompletionHandler*>(this);
				return S_OK;
			}
			else if (riid == IID_IAgileObject)
			{
				AddRef();
				*ppvObject = static_cast<IAgileObject*>(this);
				return S_OK;
			}
			*ppvObject = nullptr;
			return E_NOINTERFACE;
		}

	private:
		std::atomic<ULONG> m_refCount{ 1 };
	};

	struct LoopbackAudioCapture : LoopbackAudioCaptureT<LoopbackAudioCapture>
	{
		LoopbackAudioCapture(hstring const& renderDevice);

		ScreenSenderComponent::AudioClientBufferReadyHandler BufferReadyDelegate() const { return m_bufferReadyDelegate; }
		void BufferReadyDelegate(ScreenSenderComponent::AudioClientBufferReadyHandler const& value) { m_bufferReadyDelegate = value; }

		Windows::Media::MediaProperties::AudioEncodingProperties SuggestedEncodingProperties() const { return m_suggestedEncodingProperties; }
		void SuggestedEncodingProperties(Windows::Media::MediaProperties::AudioEncodingProperties const& value) { m_suggestedEncodingProperties = value; }

		Windows::Media::MediaProperties::AudioEncodingProperties EncodingProperties() const { return m_encodingProperties; }
		bool Started() const { return m_started; }
		int32_t SamplesPerQuantum() const { return m_samplesPerQuantum; }
		hstring Device() const { return m_device; }

		Windows::Foundation::IAsyncAction Start();
		Windows::Foundation::IAsyncAction Stop();
		void ChangeDevice(hstring const& renderDevice);

	private:
		void BeginCapture();
		void CaptureThread();

		hstring m_device;
		ScreenSenderComponent::AudioClientBufferReadyHandler m_bufferReadyDelegate{ nullptr };
		Windows::Media::MediaProperties::AudioEncodingProperties m_suggestedEncodingProperties{ nullptr };
		Windows::Media::MediaProperties::AudioEncodingProperties m_encodingProperties{ nullptr };

		bool m_started = false;
		bool m_capturing = false;
		int32_t m_samplesPerQuantum = 480;
		std::atomic<uint32_t> m_captureIndex{ 0 };

		AudioClientCallback* m_callback = nullptr;
		IAudioClient3* m_client = nullptr;
		IAudioCaptureClient* m_captureClient = nullptr;

		REFERENCE_TIME m_requestedBufferDuration = REFTIMES_PER_MILLISEC * 20;
		REFERENCE_TIME m_actualBufferDuration = 0;
		WAVEFORMATEX m_waveFormat{};
		WAVEFORMATEX m_actualWaveFormat{};
		UINT32 m_bufferFrameSize = 0;
		winrt::handle m_eventHandle;
		winrt::handle m_stoppingEvent;
	};
}

namespace winrt::ScreenSenderComponent::factory_implementation
{
	struct LoopbackAudioCapture : LoopbackAudioCaptureT<LoopbackAudioCapture, implementation::LoopbackAudioCapture>
	{
	};
}

