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

        public override void OnDespawnServer(NetworkConnection connection)
        {
            if (Owner == connection)
                RemoveOwnership();
        }

        private void OnDestroy()
        {
            PlayerManager.Unsubscribe(GiveOwnership);
        }
    }
}