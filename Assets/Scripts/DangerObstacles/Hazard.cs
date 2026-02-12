using FishNet;
using UnityEngine;
using GamePhysics;
using System;

public class Hazard : MonoBehaviour
{
    // How hard the boat gets pushed when hitting this obstacle
    [SerializeField] private float pushForce = 8f;
    [SerializeField] private int damage = 1;

    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a boat
        // Boat can be identified by BoatHealth component
        if (other.GetComponent<BoatHealth>() != null)
        {

            Debug.Log($"BOAT HIT! Obstacle at {transform.position} hit {other.gameObject.name}");

            // Passes damage dealt to script that handles boat health
            BoatHealth.Instance.TakeDamage(damage);

            // Try to push the boat away from the obstacle
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                // Calculate push direction
                Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                pushDirection.y = 0;

                // Push force
                rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                Debug.Log($"Pushed boat with force: {pushForce}");
            }

            // Destroy this obstacle after it's been hit
            if (InstanceFinder.IsServerStarted)
                InstanceFinder.ServerManager.Despawn(gameObject);
        }
    }
}