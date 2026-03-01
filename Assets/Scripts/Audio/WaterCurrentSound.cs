using UnityEngine;
using System.Collections;

public class WindSoundTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource windSound;
    [SerializeField] private float fadeDuration = 1.5f;

    private int boatInZoneCount = 0; //Keeps track how many trigger zones the boat is in

    private Coroutine  fadeCoroutine;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInZoneCount++;


            if(boatInZoneCount == 1) //Only start fading if the boat is i
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }

                fadeCoroutine = StartCoroutine(FadeVolume(0f, 1f, fadeDuration));


                //Fale safe for the sound
                if (!windSound.isPlaying)
                    windSound.Play();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boatInZoneCount--;

            if (boatInZoneCount == 0)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeVolume(windSound.volume, 0f, fadeDuration, true));
            }
        }
    }


    private IEnumerator FadeVolume(float startVol, float endVol, float duration, bool stopAtEnd = false)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            windSound.volume = Mathf.Lerp(startVol, endVol, elapsed / duration);
            yield return null;
        }
        
        windSound.volume = endVol;
        
        if (stopAtEnd && endVol == 0f)
            windSound.Stop();
            
        fadeCoroutine = null;
    }

    private void OnDisable()
    {
        boatInZoneCount = 0;
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }
}