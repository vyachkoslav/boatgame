using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(ButtonSound);
        }
    }

    public void ButtonSound()
    {
        Debug.Log("Button clicked! Playing sound.");
        SoundManager.Instance.PlaySound2D("ClickSound");
    }
}
