using System;
using FishNet.Connection;
using FishNet.Object;
using Network;
using UnityEngine;

namespace Player
{
    public class Paddle : NetworkBehaviour
    {
        [SerializeField] private bool isLeft;
        
        public override void OnStartServer()
        {
            if (isLeft)
                PlayerManager.OnLeftPlayerAssigned(GiveOwnership);
            else
                PlayerManager.OnRightPlayerAssigned(GiveOwnership);
        }

        private void OnDestroy()
        {
            PlayerManager.Unsubscribe(GiveOwnership);
        }
    }
}