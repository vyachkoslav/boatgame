using System;
using UnityEngine;

namespace Settings
{
    public enum VoiceChatMode
    {
        PushToTalk,
        AlwaysEnabled
    }
    
    [CreateAssetMenu(fileName = "Voice chat settings", menuName = "Settings/Voice chat", order = 0)]
    [Serializable]
    public class VoiceChatSettings : ScriptableObject
    {
        [Range(0, 100)] public float VolumeOut;
        [Range(0, 100)] public float VolumeIn;
        
        public VoiceChatMode Mode = VoiceChatMode.PushToTalk;
    }
}