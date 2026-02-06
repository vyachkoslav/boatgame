using System;
using System.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

namespace Network.Voice
{
    public class NetworkVivox : NetworkBehaviour
    {
        [SerializeField] private VoiceStateUI stateUI;
        private readonly SyncVar<Guid> channelGuid = new();

        public override void OnStartServer()
        {
            channelGuid.Value = Guid.NewGuid();
        }

        public override void OnStartClient()
        {
            _ = InitVivox();
        }

        public override void OnStopClient()
        {
            if (VivoxService.Instance == null) return;
            
            VivoxService.Instance.ChannelJoined -= OnJoinedChannel;
            VivoxService.Instance.ParticipantAddedToChannel -= OnMemberJoin;
            _ = VivoxService.Instance.LeaveAllChannelsAsync();
        }

        private async Task InitVivox()
        {
            await UnityServices.InitializeAsync();
            await VivoxService.Instance.InitializeAsync();
            #if UNITY_EDITOR
            VivoxService.Instance.MuteInputDevice();
            #endif

            VivoxService.Instance.ChannelJoined += OnJoinedChannel;
            VivoxService.Instance.ParticipantAddedToChannel += OnMemberJoin;
            
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            if (!VivoxService.Instance.IsLoggedIn)
                await VivoxService.Instance.LoginAsync();
            
            var channel = channelGuid.Value.ToString();
            await VivoxService.Instance.JoinGroupChannelAsync(
                channel,
                ChatCapability.AudioOnly);
        }

        private void OnJoinedChannel(string channel)
        {
            Debug.Log("Joined voice channel: " + channel);
        }

        private void OnMemberJoin(VivoxParticipant member)
        {
            if (member.IsSelf)
                stateUI.Setup(member);
        }
    }
}
