using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    public class TriggerCallback : MonoBehaviour
    {
        public UnityEvent<int, Collider> onTriggerEnter = new();
        public UnityEvent<int, Collider> onTriggerExit = new();
        public UnityEvent<int, Collider> onTriggerStay = new();

        public int id = -1;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke(id, other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit.Invoke(id, other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStay.Invoke(id, other);
        }
    }
}
