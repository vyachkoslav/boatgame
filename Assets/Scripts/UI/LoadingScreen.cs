using System.Collections;
using FishNet;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private float timeToFullOpacity = 0.5f;
        [SerializeField] private Graphic image;
        [SerializeField] private GameObject enabledAfterFullOpacity;
        
        private float currentTime;
        
        private void Start()
        {
            if (InstanceFinder.NetworkManager.HasInstance<LoadingScreen>())
            {
                Destroy(gameObject);
                return;
            }

            InstanceFinder.NetworkManager.RegisterInstance(this);
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }

        private void ShowImmediate()
        {
            gameObject.SetActive(true);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            enabledAfterFullOpacity.SetActive(true);
        }
        
        private IEnumerator Show()
        {
            gameObject.SetActive(true);
            currentTime = 0;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            enabledAfterFullOpacity?.SetActive(false);
            while (currentTime < timeToFullOpacity)
            {
                currentTime += Time.deltaTime;
                image.color = new Color(image.color.r, image.color.g, image.color.b,
                    Mathf.Clamp01(currentTime / timeToFullOpacity));
                yield return null;
            }
            enabledAfterFullOpacity?.SetActive(true);
        }

        public static void ShowLoadingScreenImmediate()
        {
            if (InstanceFinder.NetworkManager.TryGetInstance(out LoadingScreen loadingScreen))
                loadingScreen.ShowImmediate();
        }
        
        public static IEnumerator ShowLoadingScreen()
        {
            if (InstanceFinder.NetworkManager.TryGetInstance(out LoadingScreen loadingScreen))
                return loadingScreen.Show();
            return null;
        }

        public static void HideLoadingScreen()
        {
            if (InstanceFinder.NetworkManager.TryGetInstance(out LoadingScreen loadingScreen))
                loadingScreen.gameObject.SetActive(false);
        }
    }
}
