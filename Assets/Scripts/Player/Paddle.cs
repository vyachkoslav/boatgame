using FishNet.Connection;
using FishNet.Object;
using Network;
using UnityEngine;

namespace Player
{
    public class Paddle : NetworkBehaviour
    {
        [SerializeField] private bool isLeft;

        public override void OnSpawnServer(NetworkConnection connection)
        {
            connection.OnLoadedStartScenes += TransferOwnership;
        }

        private void TransferOwnership(NetworkConnection connection, bool isServer)
        {
            var type = PlayerManager.Instance.Players[connection];
            if (isLeft && type == PlayerManager.PlayerType.Left)
                GiveOwnership(connection);
            else if (!isLeft && type == PlayerManager.PlayerType.Right)
                GiveOwnership(connection);
        }

        public override void OnDespawnServer(NetworkConnection connection)
        {
            connection.OnLoadedStartScenes -= TransferOwnership;
            if (Owner == connection)
                RemoveOwnership();
        }

        private void OnDestroy()
        {
            PlayerManager.Unsubscribe(GiveOwnership);
        }
    }
}