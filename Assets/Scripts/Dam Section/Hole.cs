using UnityEngine;

public class Hole : MonoBehaviour
{
    private Collider triggerArea;
    [SerializeField] private Renderer model;
    private bool isBlocked = false;
    public HoleDam dam; // The dam that the hole belongs to

    private void Awake()
    {
        triggerArea = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // When a blocker object enters the trigger area
        if (isBlocked == false && other.gameObject.CompareTag("Blocker"))
        {
            BlockHole();

            Destroy(other.gameObject); // Removes blocker object from scene
        }
    }

    void BlockHole()
    {
        isBlocked = true;
        model.enabled = false; // Makes the hole invisible
        triggerArea.enabled = false;

        dam.RemoveHole(gameObject); // Issues removal of the game object this script is attached to
    }
}
