using System;
using FishNet.Managing;
using UnityEngine;

namespace Utility
{
    public class NetworkAutostart : MonoBehaviour
    {
        private NetworkManager networkManager;
        
        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
        }

        private void Start()
        {
            switch (NetworkUtility.Role)
            {
                case NetworkUtility.MultiplayerRole.Client:
                    networkManager.ClientManager.StartConnection();
                    break;
                case NetworkUtility.MultiplayerRole.None:
                case NetworkUtility.MultiplayerRole.Host:
                    networkManager.ServerManager.StartConnection();
                    networkManager.ClientManager.StartConnection();
                    break;
            }
        }
    }
}