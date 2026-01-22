using System;
using UnityEngine;

namespace Utility
{
    public class MainCamera : MonoBehaviour
    {
        private void Awake()
        {
            GlobalObjects.MainCamera = GetComponent<Camera>();
        }
    }
}