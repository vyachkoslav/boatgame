using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
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

        public NetworkConnection LeftPlayer { get; private set; }
        public NetworkConnection RightPlayer { get; private set; }
        
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

        private void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void OnStartServer()
        {
            LeftPlayer = FishNet.Managing.NetworkManager.EmptyConnection;
            RightPlayer = FishNet.Managing.NetworkManager.EmptyConnection;
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
                LeftPlayer = conn;
                onLeftPlayerAssigned?.Invoke(LeftPlayer);
            }
            else
            {
                Players[conn] = PlayerType.Right;
                RightPlayer = conn;
                onRightPlayerAssigned?.Invoke(RightPlayer);
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
        }
    }
}
