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
        }
        
        private IEnumerator Show()
        {
            gameObject.SetActive(true);
            yield return new WaitForSeconds(timeToFullOpacity);
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
