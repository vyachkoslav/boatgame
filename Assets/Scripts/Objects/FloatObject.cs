using UnityEngine;


public class FloatObject : MonoBehaviour
{
    [SerializeField]private float buoyancy;
    [SerializeField]private float drag;
    
    Rigidbody rb;
    int waterLayer;

    int waterContacts = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        waterLayer = LayerMask.NameToLayer("Water");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == waterLayer)
            waterContacts++;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == waterLayer)
            waterContacts--;
    }

    void FixedUpdate()
    {
        if (waterContacts > 0)
        {
            ApplyWaterForces();
        }
    }

    void ApplyWaterForces()
    {
        // Buoyancy force
        rb.AddForce(Vector3.up * buoyancy, ForceMode.Force);
        Vector3 lateralVel = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);

        // Drag force for the object to glide on water surface
        Vector3 dragForce = -lateralVel.normalized * lateralVel.sqrMagnitude * drag;
        rb.AddForce(dragForce, ForceMode.Force);
}
}
