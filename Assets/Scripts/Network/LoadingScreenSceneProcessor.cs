using UI;
using FishNet.Managing.Scened;

namespace Network
{
    public class LoadingScreenSceneProcessor : DefaultSceneProcessor
    {
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