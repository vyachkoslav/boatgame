using FishNet.Object;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoatHealthHUD : MonoBehaviour
{
    public static BoatHealthHUD Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI boatHpIndicator;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI restartText;

    private void Awake()
    {
        // Sets self as instance if it doesn't exist yet
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        boatHpIndicator.text = "Raft HP: " + BoatHealth.MaxHp;
    }

    public void UpdateBoatHp(int hp)
    {
        boatHpIndicator.text = "Raft HP: " + hp;
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over";
        string restartText = "Restarting at checkpoint";
        StartCoroutine(RestartTextRoutine(restartText));
    }

    private IEnumerator RestartTextRoutine(string text)
    {
        float delay = 1.5f;
        float typingSpeed = 0.03f;

        yield return new WaitForSeconds(delay);

        foreach (char c in text)
        {
            restartText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
