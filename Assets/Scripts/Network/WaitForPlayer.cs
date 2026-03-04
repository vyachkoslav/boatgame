using System;
using FishNet.Connection;
using FishNet.Object;
using UI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Network
{
    public class WaitForPlayer : NetworkBehaviour
    {
        private static WaitForPlayer instance;
        public static bool IsWaiting => PlayerManager.Instance == null || instance == null ||
                                        !PlayerManager.Instance.BothPlayersAssigned && !instance.spacePressed;
        
        private bool spacePressed;

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
        }
        
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && !spacePressed)
            {
                spacePressed = true;
                WaitForPlayerUI.Instance.StopWait();
            }
        }
#endif
        
        public override void OnStartClient()
        {
            spacePressed = false;
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
