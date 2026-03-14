using System.Collections;
using TMPro;
using UI;
using UnityEngine;

public class GoalInstructions : MonoBehaviour
{
    [SerializeField] private string goalText;
    [SerializeField] private TextMeshProUGUI HUDNotification;
    [SerializeField] private TutorialUI TutorialUI;

    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float visibleDuration = 5f;

    private void Start()
    {
        StartCoroutine(GoalTextRoutine(goalText));
    }

    private IEnumerator GoalTextRoutine(string text)
    {
        HUDNotification.text = "";

        yield return new WaitUntil(() => TutorialUI.IsTutorialComplete);

        yield return new WaitForSeconds(startDelay);

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
