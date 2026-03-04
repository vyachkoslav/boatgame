using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OfflineButtonClickSound : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(ButtonSound);
        }
    }

    public void ButtonSound()
    {
        Debug.Log("Button clicked! Playing sound.");
        OfflineSoundManager.Instance.PlaySound2D("ClickSound");
    }
}
