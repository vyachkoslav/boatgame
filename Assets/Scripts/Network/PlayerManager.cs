using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Assertions;

namespace Network
{
    public class PlayerManager : NetworkBehaviour
    {
        public enum PlayerType : byte
        {
            None,
            Left,
            Right,
            Spectator
        }
        
        public static PlayerManager Instance { get; private set; }

        private readonly SyncVar<NetworkConnection> leftPlayer = new();
        private readonly SyncVar<NetworkConnection> rightPlayer = new();
        public NetworkConnection LeftPlayer => leftPlayer.Value;
        public NetworkConnection RightPlayer => rightPlayer.Value;
        public bool BothPlayersAssigned => LeftPlayer.IsActive && RightPlayer.IsActive;
        
        public readonly Dictionary<NetworkConnection, PlayerType> Players = new();
        
        public PlayerType CurrentPlayer { get; private set; }

        private static Action<NetworkConnection> onLeftPlayerAssigned;
        private static Action<NetworkConnection> onRightPlayerAssigned;
        private static Action<PlayerType> onLocalPlayerAssigned;

        public static void OnLeftPlayerAssigned(Action<NetworkConnection> callback)
        {
            onLeftPlayerAssigned += callback;
            if (Instance?.LeftPlayer.IsActive == true)
                callback(Instance.LeftPlayer);
        }
        
        public static void OnRightPlayerAssigned(Action<NetworkConnection> callback)
        {
            onRightPlayerAssigned += callback;
            if (Instance?.RightPlayer.IsActive == true)
                callback(Instance.RightPlayer);
        }

        public static void OnLocalPlayerAssigned(Action<PlayerType> callback)
        {
            onLocalPlayerAssigned += callback;
            if (Instance != null && Instance.CurrentPlayer != PlayerType.None)
                callback(Instance.CurrentPlayer);
        }

        public static void Unsubscribe(Action<NetworkConnection> callback)
        {
            onLeftPlayerAssigned -= callback;
            onRightPlayerAssigned -= callback;
        }

        public static void Unsubscribe(Action<PlayerType> callback)
        {
            onLocalPlayerAssigned -= callback;
        }

        private void OnPlayerAssigned(PlayerType player, NetworkConnection conn, bool asServer)
        {
            if (asServer) return;
            switch (player)
            {
                case PlayerType.Left:
                    onLeftPlayerAssigned?.Invoke(conn);
                    break;
                case PlayerType.Right:
                    onRightPlayerAssigned?.Invoke(conn);
                    break;
            }
        }

        private void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;
            leftPlayer.SetInitialValues(FishNet.Managing.NetworkManager.EmptyConnection);
            rightPlayer.SetInitialValues(FishNet.Managing.NetworkManager.EmptyConnection);
            
            leftPlayer.OnChange += (_, next, asServer) => OnPlayerAssigned(PlayerType.Left, next, asServer);
            rightPlayer.OnChange += (_, next, asServer) => OnPlayerAssigned(PlayerType.Right, next, asServer);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void OnStartClient()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        public override void OnStopClient()
        {
            CurrentPlayer = PlayerType.None;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public override void OnSpawnServer(NetworkConnection conn)
        {
            if (LeftPlayer.IsActive && RightPlayer.IsActive)
            {
                Players[conn] = PlayerType.Spectator;
            }
            else if (!LeftPlayer.IsActive)
            {
                Players[conn] = PlayerType.Left;
                leftPlayer.Value = conn;
            }
            else
            {
                Players[conn] = PlayerType.Right;
                rightPlayer.Value = conn;
            }
            RpcSetPlayer(conn, Players[conn]);
            Debug.Log($"Assigned {conn}: {Players[conn]}");
        }

        [TargetRpc]
        private void RpcSetPlayer(NetworkConnection conn, PlayerType type)
        {
            CurrentPlayer = type;
            onLocalPlayerAssigned?.Invoke(type);
        }

        public override void OnDespawnServer(NetworkConnection conn)
        {
            Debug.Log("Despawn " + conn);
            Players.Remove(conn, out var type);
            if (type == PlayerType.Left)
                leftPlayer.Value = FishNet.Managing.NetworkManager.EmptyConnection;
            else if (type == PlayerType.Right)
                rightPlayer.Value = FishNet.Managing.NetworkManager.EmptyConnection;
        }
    }
}
