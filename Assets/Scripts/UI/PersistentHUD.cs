using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersistentHUD : MonoBehaviour
{
    public static PersistentHUD Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI boatHpIndicator;

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
}
