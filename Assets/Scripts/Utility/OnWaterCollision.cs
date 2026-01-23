using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    public class OnWaterCollision : MonoBehaviour
    {
        private static int waterLayer;
        
        public UnityEvent OnWaterCollisionEnter = new();
        public UnityEvent OnWaterCollisionExit = new();

        private void Awake()
        {
            waterLayer = LayerMask.NameToLayer("Water");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == waterLayer)
                OnWaterCollisionEnter.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == waterLayer)
                OnWaterCollisionExit.Invoke();
        }
    }
}