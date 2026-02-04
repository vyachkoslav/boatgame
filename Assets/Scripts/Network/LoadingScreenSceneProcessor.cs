using System;
using UI;
using FishNet.Managing.Scened;
using UnityEngine;

namespace Network
{
    public class LoadingScreenSceneProcessor : DefaultSceneProcessor
    {
        [SerializeField] private float minLoadingTime = 1f;
        private float timeToDisable;
        private bool ended;
        
        public override void LoadStart(LoadQueueData queueData)
        {
            base.LoadStart(queueData);
            LoadingScreen.ShowLoadingScreen();
            timeToDisable = minLoadingTime;
        }

        public override void LoadEnd(LoadQueueData queueData)
        {
            base.LoadEnd(queueData);
            ended = timeToDisable > 0;
            if (timeToDisable <= 0)
                LoadingScreen.HideLoadingScreen();
        }

        private void Update()
        {
            timeToDisable -= Time.deltaTime;
            if (timeToDisable <= 0 && ended)
            {
                LoadingScreen.HideLoadingScreen();
                ended = false;
            }
        }
    }
}