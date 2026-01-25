using UnityEngine;

public class Hole : MonoBehaviour
{
    private Collider triggerArea;
    private Renderer model;

    private void Awake()
    {
        triggerArea = GetComponent<Collider>();
        model = GetComponentInChildren<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Makes the hole invisible when a blocker is moved close enough
        //if (other.gameObject.CompareTag("Blocker"))
        //{
        //    model.enabled = false;
        //}
    }
}
