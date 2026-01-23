using System;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class GrabBoth : MonoBehaviour
    {
        [SerializeField] private InputActionReference grabAction;
        [SerializeField] private Paddle first;
        [SerializeField] private Paddle second;

        private void OnEnable()
        {
            grabAction.action.started += Grab;
            grabAction.action.canceled += Ungrab;
        }

        private void Grab(InputAction.CallbackContext callbackContext)
        {
            first.Grab();
            second.Grab();
        }

        private void Ungrab(InputAction.CallbackContext callbackContext)
        {
            first.Ungrab();
            second.Ungrab();
        }
    }
}