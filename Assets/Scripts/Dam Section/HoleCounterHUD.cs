using UnityEngine;
using TMPro;

public class HoleCounterHUD : MonoBehaviour
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

    public void UpdateHoleCount(int holeCount)
    {
        if (holeCountIndicator != null)
            holeCountIndicator.text = "Holes Remaining: " + holeCount;
    }

    public void ClearHoleCount()
    {
        if (holeCountIndicator != null)
            holeCountIndicator.text = "";
    }
}