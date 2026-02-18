using System.Collections.Generic;
using UnityEngine;

namespace GamePhysics
{
    [RequireComponent(typeof(Rigidbody))]
    public class OfflineFloatingObject : MonoBehaviour
    {
        [SerializeField] private List<Transform> supportPoints = new();
        [SerializeField] private float yOffset;
        [SerializeField] private float buoyancyStrength = 20f;
        [SerializeField] private float damping = 2f;
        
        private Rigidbody rb;
        private IWater water;

        private void OnValidate()
        {
            supportPoints.Clear();
            InitSupports(transform);
        }

        private void InitSupports(Transform tr)
        {
            foreach (Transform child in tr)
            {
                if (child.gameObject.CompareTag("Support"))
                    supportPoints.Add(child);
                else 
                    InitSupports(child);
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            water = GetComponent<IWater>();
            
            if (water == null)
            {
                // Try to find water in the scene if not attached to this object
                water = FindFirstObjectByType<OfflineWater>();
                
                if (water == null)
                {
                    Debug.LogError("OfflineFloatingObject: No IWater component found in scene!", this);
                }
            }
        }

        private void FixedUpdate()
        {
            if (water == null) return;
            
            foreach (var point in supportPoints)
            {
                if (point == null) continue;
                
                var pos = point.position;
                var waterLevel = water.GetWaterPointHeight(pos);
                var depth = (waterLevel - pos.y) + yOffset;
                
                if (depth <= 0f) continue;
                
                var velocity = rb.GetPointVelocity(pos);
                var forceMagnitude = depth * buoyancyStrength - velocity.y * damping;
                var force = Vector3.up * forceMagnitude;
                
                rb.AddForceAtPosition(force, pos, ForceMode.Acceleration);
            }
        }
    }
}