using System;
using System.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;
using Utility;

namespace Network.Voice
{
    public class NetworkVivox : NetworkBehaviour
    {
        [SerializeField] private VoiceStateUI stateUI;

        [SerializeField] private GameObject leftHead;
        [SerializeField] private GameObject rightHead;
        private GameObject thisHead;
        
        private readonly SyncVar<Guid> channelGuid = new();
        private string channelName;
        
        private NetworkConnection leftPlayer;
        private NetworkConnection rightPlayer;
        
        [Header("Vivox settings")]
        [SerializeField] private int audibleDistance = 20;
        [SerializeField] private int conversationalDistance = 5;
        [SerializeField] private float audioFadeIntensityByDistance = 0.5f;
        [SerializeField] private AudioFadeModel audioFadeModel = AudioFadeModel.LinearByDistance;
        

        public override void OnStartServer()
        {
            channelGuid.Value = Guid.NewGuid();
            NetworkManager.ServerManager.OnRemoteConnectionState += OnClientState;
        }

        public override void OnStopServer()
        {
            NetworkManager.ServerManager.OnRemoteConnectionState -= OnClientState;
        }

        public override void OnSpawnServer(NetworkConnection conn)
        {
            RpcPlayerSeat(conn, conn == leftPlayer);
        }

        private void OnClientState(NetworkConnection conn, RemoteConnectionStateArgs args)
        {
            if (args.ConnectionState == RemoteConnectionState.Stopped)
            {
                if (leftPlayer == conn) leftPlayer = null;
                else if (rightPlayer == conn) rightPlayer = null;
                else Debug.LogError("Client without seat disconnected");
                return;
            }

            if (leftPlayer == null) rightPlayer = conn;
            else if (rightPlayer == null) leftPlayer = conn;
            else Debug.LogError("No place for new client");
        }

        [TargetRpc]
        private void RpcPlayerSeat(NetworkConnection target, bool isLeft)
        {
            _ = InitVivox(isLeft ? leftHead : rightHead);
        }

        public override void OnStopClient()
        {
            VivoxService.Instance.ChannelJoined -= OnJoinedChannel;
            VivoxService.Instance.ParticipantAddedToChannel -= OnMemberJoin;
        }

        private async Task InitVivox(GameObject player)
        {
            thisHead = player;
            
            await UnityServices.InitializeAsync();
            await VivoxService.Instance.InitializeAsync();
            #if UNITY_EDITOR
            VivoxService.Instance.MuteInputDevice();
            #endif

            VivoxService.Instance.ChannelJoined += OnJoinedChannel;
            VivoxService.Instance.ParticipantAddedToChannel += OnMemberJoin;
            
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await VivoxService.Instance.LoginAsync();
            
            var channel = channelGuid.Value.ToString();
            await VivoxService.Instance.JoinPositionalChannelAsync(
                channel,
                ChatCapability.AudioOnly,
                new Channel3DProperties(audibleDistance, conversationalDistance, 
                                        audioFadeIntensityByDistance, audioFadeModel));
            channelName = channel;
        }

        private void Update()
        {
            if (channelName == null) return;
            
            VivoxService.Instance.Set3DPosition(
                thisHead.transform.position, 
                GlobalObjects.MainCamera.transform.position,
                GlobalObjects.MainCamera.transform.forward,
                Vector3.up, 
                channelName, true);
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
