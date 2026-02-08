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
        private Task initTask;

        public override void OnStartServer()
        {
            channelGuid.Value = Guid.NewGuid();
        }

        public override void OnStartClient()
        {
            initTask ??= InitVivox();
        }

        public override void OnStopClient()
        {
            if (VivoxService.Instance != null && VivoxService.Instance.IsLoggedIn)
                _ = VivoxService.Instance.LeaveAllChannelsAsync();
        }

        private void OnDestroy()
        {
            if (VivoxService.Instance == null) return;
            
            VivoxService.Instance.ChannelJoined -= OnJoinedChannel;
            VivoxService.Instance.ParticipantAddedToChannel -= OnMemberJoin;
            VivoxService.Instance.LoggedIn -= OnLoggedIn;
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
            VivoxService.Instance.LoggedIn += OnLoggedIn;
            
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            if (!VivoxService.Instance.IsLoggedIn)
                await VivoxService.Instance.LoginAsync();
        }

        private void OnLoggedIn()
        {
            if (IsClientStarted)
                _ = JoinChannel();
        }

        private async Task JoinChannel()
        {
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
