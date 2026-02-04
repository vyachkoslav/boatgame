using System;
using UI;
using FishNet.Managing.Scened;
using UnityEngine;

namespace Network
{
    public class LoadingScreenSceneProcessor : DefaultSceneProcessor
    {
        [SerializeField] private float minLoadingTime = 1f;
        
        public override void LoadStart(LoadQueueData queueData)
        {
            base.LoadStart(queueData);
            LoadingScreen.ShowLoadingScreen();
        }

        public override void LoadEnd(LoadQueueData queueData)
        {
            base.LoadEnd(queueData);
            LoadingScreen.HideLoadingScreen();
        }
    }
}