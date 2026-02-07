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
    }
}