using TMPro;
using UnityEngine;

public class Dam : MonoBehaviour
{
    [SerializeField] protected string puzzleInstructions;
    protected TextMeshProUGUI HUDNotification;
    protected bool boatDetected = false;

    protected void Awake()
    {
        //HUDNotification = GameObject.Find("").gameObject.GetComponent<TextMeshProUGUI>;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (boatDetected == false && other.CompareTag("Boat"))
        {
            boatDetected = true;

            StartPuzzle();
        }
    }

    protected virtual void StartPuzzle()
    {
        //HUDNotification.text = puzzleInstructions;
    }

    protected virtual void EndPuzzle()
    {
        Debug.Log("Puzzle solved!");
        //HUDNotification.text = "Puzzle solved!";
    }
}
