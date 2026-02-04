using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LoadingText : MonoBehaviour
    {
        [SerializeField] private float timeBetweenUpdates = 0.8f;
        
        private TextMeshProUGUI label;
        private Coroutine textChangeRoutine;
        
        private void Awake()
        {
            label = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            label.text = "Loading.";
            textChangeRoutine = StartCoroutine(TextChange());
        }

        private void OnDisable()
        {
            StopCoroutine(textChangeRoutine);
        }

        private IEnumerator TextChange()
        {
            var wait = new WaitForSeconds(timeBetweenUpdates);
            while (isActiveAndEnabled)
            {
                yield return wait;
                label.text = "Loading..";
                yield return wait;
                label.text = "Loading...";
                yield return wait;
                label.text = "Loading.";
            } 
        }
    }
}
