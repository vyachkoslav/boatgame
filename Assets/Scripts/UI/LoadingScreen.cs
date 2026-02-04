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
            HideLoadingScreen();
        }

        private void OnEnable()
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            enabledAfterFullOpacity?.SetActive(false);
        }

        private void Update()
        {
            if (currentTime > timeToFullOpacity)
            {
                enabledAfterFullOpacity?.SetActive(true);
                return;
            }
            
            currentTime += Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 
                Mathf.Clamp01(currentTime/timeToFullOpacity));
        }

        public static void ShowLoadingScreen()
        {
            if (InstanceFinder.NetworkManager.TryGetInstance(out LoadingScreen loadingScreen))
                loadingScreen.gameObject.SetActive(true);
        }

        public static void HideLoadingScreen()
        {
            if (InstanceFinder.NetworkManager.TryGetInstance(out LoadingScreen loadingScreen))
                loadingScreen.gameObject.SetActive(false);
        }
    }
}
