using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WaterCurrent : MonoBehaviour
{
    [SerializeField] private float speed = 15.0f;
    private float indicatorMultiplier = 0.01f;
    private Vector3 direction;
    private bool isPushing = false;

    [SerializeField] private float length = 1.0f; // Z
    [SerializeField] private float width = 1.0f; // X
    [SerializeField] private float rotationY;

    Material material;

    private List<Rigidbody> objectsInCurrent = new List<Rigidbody>();

    

    private void Awake()
    {
        direction = transform.forward;
        material = GetComponentInChildren<MeshRenderer>().material;
    }

    private void OnValidate()
    {
        UpdateTransform();
    }

    private void FixedUpdate()
    {
        // Moves the texture of direction indicator
        material.mainTextureOffset += new Vector2(0, 1) * speed * indicatorMultiplier * Time.fixedDeltaTime;

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

    // Updates object scale and forward direction according to changes in inspector
    void UpdateTransform()
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
