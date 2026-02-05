using FishNet;
using UnityEngine;
using GamePhysics;

public class RiverObstacle : MonoBehaviour
{
    // How hard the boat gets pushed when hitting this obstacle
    [SerializeField] private float pushForce = 8f;
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a boat
        // Boats can be identified by "Player" tag or FloatingObject component
        if (other.CompareTag("Player") || other.GetComponent<FloatingObject>() != null)
        {
           
            Debug.Log($"BOAT HIT! Obstacle at {transform.position} hit {other.gameObject.name}");
            
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