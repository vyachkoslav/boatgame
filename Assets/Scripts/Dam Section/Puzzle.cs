using TMPro;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] protected string puzzleInstructions;

    //Timer and HUD references
    protected TextMeshProUGUI HUDNotification;
    [SerializeField] protected TextMeshProUGUI TimerText;
    [SerializeField] protected GameObject TimerBox;
    [SerializeField] protected float startingTime = 60f;
    protected float currentTime;

    //Game States
    protected bool boatEntered = false;
    protected bool timerRunning = false;


    protected void Awake()
    {
        HUDNotification = GameObject.Find("HUDNotification")
            ?.GetComponent<TextMeshProUGUI>();
    }

    protected void Update()
    {
        if (!timerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            timerRunning = false;

            OnTimeExpired();
        }
        UpdateTimerDisplay();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (boatEntered) return;
        if (boatEntered == false && other.CompareTag("Boat"))
        {
            boatEntered = true;

            StartTime();
            ShowInstructions();
            StartPuzzle();
        }
    }

    protected void ShowInstructions() //Handles displaying the puzzle instructions on the HUD
    {
        if (HUDNotification != null)
        {
            HUDNotification.text = puzzleInstructions;
        }
    }

    protected void StartTime() //Handles starting the timer display on the HUD
    {
        Debug.Log("Starting timer");
        if (TimerBox != null)
        {
            TimerBox.SetActive(true);
        }
        currentTime = startingTime;
        timerRunning = true;
        UpdateTimerDisplay();
    }

    protected void UpdateTimerDisplay()
    {
        if (TimerText != null)
        {
            TimerText.text = currentTime.ToString("F1");
        }
    }
    protected virtual void OnTimeExpired()
    {
        Debug.Log("Time expired!");

        //Add the logic for what happens when time expires (the dam breaks)
    }

    protected virtual void StartPuzzle()
    {
        Debug.Log("Puzzle started!");
    }

    protected virtual void EndPuzzle()
    {
        Debug.Log("Puzzle solved!");
        if(HUDNotification != null)
        {
            HUDNotification.text = "Puzzle solved!";
        }
        timerRunning = false;
        TimerBox.SetActive(false);
    }
}
