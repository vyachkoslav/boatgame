using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private void Start()
    {
        // Validate references
        if (targetText == null)
        {
            Debug.LogError("Target TextMeshProUGUI is not assigned!");
            return;
        }
        
        if (hostButton == null)
        {
            Debug.LogError("Host Button is not assigned!");
            return;
        }
        
        if (joinButton == null)
        {
            Debug.LogError("Join Button is not assigned!");
            return;
        }
        
    }
    
    public void SetHostText()
    {
        targetText.text = "Host";
    }
    

    public void SetJoinText()
    {
        targetText.text = "Join";
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}