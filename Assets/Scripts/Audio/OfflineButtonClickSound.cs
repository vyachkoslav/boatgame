using UnityEngine;
using UnityEngine.UI;

public class OfflineButtonClickSound : MonoBehaviour
{
    public void ButtonSound()
    {
        Debug.Log("Button clicked! Playing sound.");
        OfflineSoundManager.Instance.PlaySound2D("ClickSound");
    }
}
