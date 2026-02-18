using UnityEngine;

public class EndZoneDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MenuFloatingObject"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
