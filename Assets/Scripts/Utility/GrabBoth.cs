using System;
using FishNet;
using Network;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class GrabBoth : MonoBehaviour
    {
        [SerializeField] private InputActionReference grabAction;
        [SerializeField] private Paddle left;
        [SerializeField] private Paddle right;

        private Paddle GetOtherPaddle()
        {
            return PlayerManager.Instance.CurrentPlayer switch
            {
                PlayerManager.PlayerType.Left => right,
                PlayerManager.PlayerType.Right => left,
                _ => null
            };
        }

        private void Grab(InputAction.CallbackContext callbackContext)
        {
            if (!InstanceFinder.IsHostStarted || PlayerManager.Instance == null) return;

            var other = GetOtherPaddle();
            if (other == null || other.Owner.IsActive) return;
            other.GiveOwnership(InstanceFinder.ClientManager.Connection);
        }

        private void Ungrab(InputAction.CallbackContext callbackContext)
        {
            if (!InstanceFinder.IsHostStarted || PlayerManager.Instance == null) return;
            
            var other = GetOtherPaddle();
            if (other == null || !other.Owner.IsLocalClient) return;
            other.RemoveOwnership();
        }
        
        private void OnEnable()
        {
            grabAction.action.started += Grab;
            grabAction.action.canceled += Ungrab;
        }

        private void OnDisable()
        {
            grabAction.action.started -= Grab;
            grabAction.action.canceled -= Ungrab;
        }
    }
}