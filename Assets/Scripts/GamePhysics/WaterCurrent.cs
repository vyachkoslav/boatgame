using System.Collections.Generic;
using UnityEngine;

namespace GamePhysics
{
    public class WaterCurrent : MonoBehaviour
    {
        private const float IndicatorMultiplier = 0.01f;
    
        [SerializeField] private float speed = 15.0f;
        [SerializeField] private float length = 1.0f; // Z
        [SerializeField] private float width = 1.0f; // X
        [SerializeField] private float rotationY;

        [SerializeField] private new Renderer renderer;
    
        private readonly List<Rigidbody> objectsInCurrent = new();

        private Material material;
        private Vector3 direction;

        private void Awake()
        {
            direction = transform.forward;
            material = renderer.material;
        }

        private void OnValidate()
        {
            UpdateTransform();
        }

        private void Update()
        {
            // Moves the texture of direction indicator
            material.mainTextureOffset += new Vector2(0, speed * IndicatorMultiplier * Time.deltaTime);
        }
    
        private void FixedUpdate()
        {
            // Pushes all objects within the water current in the current's direction
            foreach (Rigidbody rb in objectsInCurrent)
            {
                rb.AddForce(direction * speed);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Detects objects in the water current and places them in the list
            if (other.gameObject.TryGetComponent(out Rigidbody rb))
            {
                objectsInCurrent.Add(rb);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // When an object leaves the current, it is removed from the list
            if (other.gameObject.TryGetComponent(out Rigidbody rb))
            {
                objectsInCurrent.Remove(rb);
            }
        }

        // Updates object scale and forward direction according to changes in inspector
        private void UpdateTransform()
        {
            Vector3 scale = transform.localScale;
            scale.z = length;
            scale.x = width;
            transform.localScale = scale;

            Vector3 rotation = transform.eulerAngles;
            rotation.y = rotationY;
            transform.eulerAngles = rotation;

            direction = transform.forward;
        }
    }
}
