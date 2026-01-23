using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WaterCurrent : MonoBehaviour
{
    [SerializeField] private float speed = 15.0f;
    private Vector3 direction;
    private bool isPushing = false;
    private List<Rigidbody> objectsInCurrent = new List<Rigidbody>();

    [SerializeField] private float length = 1.0f; // Z
    [SerializeField] private float width = 1.0f; // X

    private void Awake()
    {
        direction = transform.forward;
    }

    private void OnValidate()
    {
        UpdateScale();
    }

    private void FixedUpdate()
    {
        if (isPushing & objectsInCurrent.Count != 0)
        {
            // Pushes all objects within the water current in the current's direction
            foreach (Rigidbody rb in objectsInCurrent)
            {
                rb.AddForce(direction * speed);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detects objects in the water current and places them in the list
        if (other.gameObject.TryGetComponent(out Rigidbody rb))
        {
            objectsInCurrent.Add(rb);
            isPushing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When an object leaves the current, it is removed from the list
        if (other.gameObject.TryGetComponent(out Rigidbody rb))
        {
            objectsInCurrent.Remove(rb);

            // Checks if there are no more objects left in the list
            if (objectsInCurrent.Count == 0 )
            {
                isPushing = false;
            }
        }
    }

    // Updates object scale according to changes in inspector
    void UpdateScale()
    {
        Vector3 scale = transform.localScale;
        scale.z = length;
        scale.x = width;
        transform.localScale = scale;
    }
}
