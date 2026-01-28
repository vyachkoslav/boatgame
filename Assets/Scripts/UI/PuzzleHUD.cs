using System;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class PuzzleHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI HUDNotification;
        [SerializeField] private TextMeshProUGUI TimerText;
        [SerializeField] private GameObject TimerBox;

        private float timerTime;

        private void Awake()
        {
            GlobalObjects.PuzzleHUD = this;
        }

        public void ShowNotification(string text)
        {
            HUDNotification.text = text;
        }

        public void ShowTimer()
        {
            TimerBox.SetActive(true);
        }
        
        public void UpdateTimer(float seconds)
        {
            TimerText.text = seconds.ToString("F1");
        }

        public void HideTimer()
        {
            TimerBox.SetActive(false);
        }
    }
}
