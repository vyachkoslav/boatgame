using System;
using Unity.Multiplayer.PlayMode;

namespace Utility
{
    public static class NetworkUtility
    {
        public enum MultiplayerRole
        {
            None,
            Client,
            Host,
        }
        
        #if UNITY_EDITOR
        public static MultiplayerRole Role => GetPlayModeType();
        private static MultiplayerRole GetPlayModeType()
        {
            if (CurrentPlayer.Tags.Count == 0)
                return MultiplayerRole.None;
            
            return CurrentPlayer.Tags[0] switch
            {
                "Client" => MultiplayerRole.Client,
                "Host" => MultiplayerRole.Host,
                _ => throw new ArgumentException("Unexpected multiplayer tag: " + CurrentPlayer.Tags[0])
            };
        }
        #endif
    }
}