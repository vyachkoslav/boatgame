using System;
using TMPro;
using UnityEngine;
using Utility;
using System.Collections;

namespace UI
{
    public class PuzzleHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI HUDNotification;
        [SerializeField] private TextMeshProUGUI TimerText;
        [SerializeField] private GameObject TimerBox;
        [Header("Text Settings")]
        [SerializeField] private float typingSpeed = 0.04f;
        [SerializeField] private float visibleDuration = 2.5f;


        private Coroutine notificationRoutine;
        private float timerTime;

        private void Awake()
        {
            GlobalObjects.PuzzleHUD = this;
            HUDNotification.text = "";
        }

        public void ShowNotification(string text)
        {
            if (notificationRoutine != null)
                StopCoroutine(notificationRoutine);
            notificationRoutine = StartCoroutine(ShowNotificationRoutine(text));
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




        private IEnumerator ShowNotificationRoutine(string text)
        {
            HUDNotification.text = "";
            

            //Type out the text slowly
            foreach (char c in text)
            {
                HUDNotification.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }

            //Wait for a little while so the player can read it
            yield return new WaitForSeconds(visibleDuration);

            //Hide the text slowly
            yield return StartCoroutine(HideNotificationRoutine());

            notificationRoutine = null;
        }

        private IEnumerator HideNotificationRoutine()
        {
            while (HUDNotification.text.Length > 0)
            {
                HUDNotification.text = HUDNotification.text.Substring(0, HUDNotification.text.Length - 1);
                yield return new WaitForSeconds(typingSpeed);
            }
        }


    }
}
