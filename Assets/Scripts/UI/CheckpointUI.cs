using FishNet;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CheckpointUI : MonoBehaviour
    {
        [SerializeField] private Graphic graphic;
        [SerializeField] private float timeToOpacity = 1f;
        [SerializeField] private float timeToDisplay = 5f;

        private float displayTime;
        
        private void Start()
        {
            if (InstanceFinder.NetworkManager.HasInstance<CheckpointUI>())
            {
                Destroy(gameObject);
                return;
            }

            InstanceFinder.NetworkManager.RegisterInstance(this);
            DontDestroyOnLoad(gameObject);
            Hide();
        }

        private void OnEnable()
        {
            displayTime = 0;
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1f);
        }

        private void Update()
        {
            if (displayTime > timeToDisplay)
            {
                Hide();
                return;
            }
            displayTime += Time.deltaTime;

            var periods = displayTime / timeToOpacity;
            var currentPeriod = periods - (int)periods;
            var increasing = (int)periods % 2 == 1;
            if (!increasing) currentPeriod = 1 - currentPeriod;
            
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, currentPeriod);
        }
        
        public static void Reached()
        {
            if (InstanceFinder.NetworkManager.TryGetInstance(out CheckpointUI ui))
                ui.gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
