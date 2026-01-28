using UnityEngine;

public class Hole : MonoBehaviour
{
    public HoleDam dam; // The dam that the hole belongs to

    private void OnTriggerEnter(Collider other)
    {
        // When a blocker object enters the trigger area
        if (other.gameObject.CompareTag("Blocker"))
        {
            dam?.RemoveHole(gameObject, other.gameObject); // Issues removal of the game object this script is attached to
        }
    }
}
