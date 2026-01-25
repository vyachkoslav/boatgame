using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

namespace Network.Voice
{
    public class VoiceStateUI : MonoBehaviour
    {
        // Player specific items.
        public VivoxParticipant Participant;

        public Image ChatStateImage;
        public Sprite MutedImage;
        public Sprite SpeakingImage;
        public Sprite NotSpeakingImage;
        public Slider ParticipantVolumeSlider;
        public Button MuteButton;

        const float k_minSliderVolume = -50;
        const float k_maxSliderVolume = 7;
        readonly Color k_MutedColor = new Color(1, 0.624f, 0.624f, 1);

        private void UpdateChatStateImage()
        {
            if (VivoxService.Instance.IsInputDeviceMuted)
            {
                ChatStateImage.sprite = MutedImage;
                ChatStateImage.gameObject.transform.localScale = Vector3.one;
            }
            else
            {
                if (Participant.SpeechDetected)
                {
                    ChatStateImage.sprite = SpeakingImage;
                    ChatStateImage.gameObject.transform.localScale = Vector3.one;
                }
                else
                {
                    ChatStateImage.sprite = NotSpeakingImage;
                }
            }
        }

        public void Setup(VivoxParticipant participant)
        {
            Participant = participant;
            UpdateChatStateImage();
            Participant.ParticipantSpeechDetected += UpdateChatStateImage;

            MuteButton.onClick.AddListener(() =>
            {
                if (VivoxService.Instance.IsInputDeviceMuted)
                    VivoxService.Instance.UnmuteInputDevice();
                else
                    VivoxService.Instance.MuteInputDevice();
                UpdateChatStateImage();
            });

            ParticipantVolumeSlider.minValue = k_minSliderVolume;
            ParticipantVolumeSlider.maxValue = k_maxSliderVolume;
            ParticipantVolumeSlider.value = VivoxService.Instance.OutputDeviceVolume;
            ParticipantVolumeSlider.onValueChanged.AddListener(OnParticipantVolumeChanged);
        }

        void OnDestroy()
        {
            if (Participant != null)
            {
                Participant.ParticipantMuteStateChanged -= UpdateChatStateImage;
                Participant.ParticipantSpeechDetected -= UpdateChatStateImage;
            }

            MuteButton.onClick.RemoveAllListeners();
            ParticipantVolumeSlider.onValueChanged.RemoveAllListeners();
        }

        void OnParticipantVolumeChanged(float volume)
        {
            VivoxService.Instance.SetOutputDeviceVolume((int)volume);
        }
    }
}
