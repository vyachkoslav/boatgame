using TMPro;
using UnityEngine;

public class Dam : MonoBehaviour
{
    [SerializeField] string puzzleInstructions;
    [SerializeField] TextMeshProUGUI HUD;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Boat"))
        //{
        //    Debug.Log(other.name + " detected");
        //    HUD.text = puzzleInstructions;
        //}
    }
}
