using UnityEngine;
using TMPro;
using FishNet.Object;

public class HoleCounterHUD : NetworkBehaviour
{
    public static HoleCounterHUD Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI holeCountIndicator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Initialize with empty text
        holeCountIndicator.text = "";
    }

    [ObserversRpc]
    public void UpdateHoleCount(int holeCount)
    {
        if (holeCountIndicator != null)
            holeCountIndicator.text = "Holes Remaining: " + holeCount;
    }

    [ObserversRpc]
    public void ClearHoleCount()
    {
        if (holeCountIndicator != null)
            holeCountIndicator.text = "";
    }
}