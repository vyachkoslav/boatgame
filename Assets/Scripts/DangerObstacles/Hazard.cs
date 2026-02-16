using FishNet;
using UnityEngine;
using FishNet.Object;
using UnityEngine.VFX;

public class Hazard : NetworkBehaviour
{
    // How hard the boat gets pushed when hitting this obstacle
    [SerializeField] private float pushForce = 8f;
    [SerializeField] private int damage = 1;

    [Header("Visual Effects")]
    [SerializeField] private GameObject explosionVFXPrefab;
    [SerializeField] private float vfxDestroyDelay = 1f;



    void OnTriggerEnter(Collider other)
    {
        if (IsClientOnlyStarted) return;
        
        // Check if the colliding object is a boat
        // Boat can be identified by BoatHealth component
        if (other.GetComponent<BoatHealth>() != null)
        {

            Debug.Log($"BOAT HIT! Obstacle at {transform.position} hit {other.gameObject.name}");


            RpcSpawnExplosionVFX();

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
            InstanceFinder.ServerManager.Despawn(gameObject);
        }
    }


    [ObserversRpc]
    private void RpcSpawnExplosionVFX()
    {
        if (explosionVFXPrefab != null)
        {
            // Instantiate the explosion prefab at the bomb's position
            GameObject explosion = Instantiate(explosionVFXPrefab, transform.position, transform.rotation);
            
            //Trigger effect via event
            VisualEffect vfx = explosion.GetComponent<VisualEffect>();
            vfx.SendEvent("Explosion"); // Send event to start the effect
            
            // Destroy the explosion after it finishes playing
            Destroy(explosion, vfxDestroyDelay);
            
            Debug.Log($"Explosion VFX spawned at {transform.position}");
        }
        else
        {
            Debug.LogWarning("No explosion VFX prefab assigned to Hazard!");
        }
    }


}