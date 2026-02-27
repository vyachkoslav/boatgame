using System;
using FishNet.Connection;
using FishNet.Object;
using UI;
using UnityEngine.Assertions;

namespace Network
{
    public class WaitForPlayer : NetworkBehaviour
    {
        private static WaitForPlayer instance;
        public static bool IsWaiting => !PlayerManager.Instance.BothPlayersAssigned;

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
        }
        
        public override void OnStartClient()
        {
            PlayerManager.OnLeftPlayerAssigned(OnPlayerAssigned);
            PlayerManager.OnRightPlayerAssigned(OnPlayerAssigned);
        }

        public override void OnStopClient()
        {
            PlayerManager.Unsubscribe(OnPlayerAssigned);
            WaitForPlayerUI.Instance.StopWait();
        }

        private void OnPlayerAssigned(NetworkConnection conn)
        {
            if (PlayerManager.Instance.BothPlayersAssigned)
                WaitForPlayerUI.Instance.StopWait();
            else
                WaitForPlayerUI.Instance.StartWait();
        }
    }
}
