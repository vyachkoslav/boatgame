using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using UI;
using FishNet.Managing.Scened;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;
using UnityEngine;

namespace Network
{
    public class LoadingScreenSceneProcessor : SceneProcessorBase
    {
        #region Private.
        protected List<AsyncOperation> LoadingAsyncOperations = new();
        protected List<UnityScene> Scenes = new();
        protected AsyncOperation CurrentAsyncOperation;
        private UnityScene _lastLoadedScene;
        private Coroutine loadingScreenCoroutine;
        private bool scenesActivated = false;
        #endregion

        public override void LoadStart(LoadQueueData queueData)
        {
            ResetValues();
        }

        public override void LoadEnd(LoadQueueData queueData)
        {
            ResetValues();
            if (loadingScreenCoroutine != null)
            {
                StopCoroutine(loadingScreenCoroutine);
                loadingScreenCoroutine = null;
            }
            LoadingScreen.HideLoadingScreen();
        }
        
        /// <summary>
        /// Resets values for a fresh load or unload.
        /// </summary>
        private void ResetValues()
        {
            CurrentAsyncOperation = null;
            LoadingAsyncOperations.Clear();
            scenesActivated = false;
        }

        /// <summary>
        /// Called when scene unloading has begun within an unload operation.
        /// </summary>
        /// <param name = "queueData"></param>
        public override void UnloadStart(UnloadQueueData queueData)
        {
            LoadingScreen.ShowLoadingScreenImmediate();
            Scenes.Clear();
        }

        public override void UnloadEnd(UnloadQueueData queueData)
        {
            LoadingScreen.HideLoadingScreen();
        }

        /// <summary>
        /// Begin loading a scene using an async method.
        /// </summary>
        /// <param name = "sceneName">Scene name to load.</param>
        public override void BeginLoadAsync(string sceneName, UnityEngine.SceneManagement.LoadSceneParameters parameters)
        {
            loadingScreenCoroutine = StartCoroutine(LoadScene(sceneName, parameters));
        }

        /// <summary>
        /// Begin unloading a scene using an async method.
        /// </summary>
        /// <param name = "sceneName">Scene name to unload.</param>
        public override void BeginUnloadAsync(UnityScene scene)
        {
            CurrentAsyncOperation = UnitySceneManager.UnloadSceneAsync(scene);
        }

        /// <summary>
        /// Returns if a scene load or unload percent is done.
        /// </summary>
        /// <returns></returns>
        public override bool IsPercentComplete()
        {
            return GetPercentComplete() >= 0.9f;
        }

        /// <summary>
        /// Returns the progress on the current scene load or unload.
        /// </summary>
        /// <returns></returns>
        public override float GetPercentComplete()
        {
            if (loadingScreenCoroutine != null)
                return 0f;
            return CurrentAsyncOperation == null ? 1f : CurrentAsyncOperation.progress;
        }

        /// <summary>
        /// Gets the scene last loaded by the processor.
        /// </summary>
        /// <remarks>This is called after IsPercentComplete returns true.</remarks>
        public override UnityScene GetLastLoadedScene() => _lastLoadedScene;

        /// <summary>
        /// Adds a loaded scene.
        /// </summary>
        /// <param name = "scene">Scene loaded.</param>
        public override void AddLoadedScene(UnityScene scene)
        {
            base.AddLoadedScene(scene);
            Scenes.Add(scene);
        }

        /// <summary>
        /// Returns scenes which were loaded during a load operation.
        /// </summary>
        public override List<UnityScene> GetLoadedScenes()
        {
            return Scenes;
        }

        /// <summary>
        /// Activates scenes which were loaded.
        /// </summary>
        public override void ActivateLoadedScenes()
        {
            scenesActivated = true;
            for (int i = 0; i < LoadingAsyncOperations.Count; i++)
            {
                try
                {
                    LoadingAsyncOperations[i].allowSceneActivation = true;
                }
                catch (Exception e)
                {
                    SceneManager.NetworkManager.LogError($"An error occured while activating scenes. {e.Message}");
                }
            }
        }

        private IEnumerator LoadScene(string sceneName, UnityEngine.SceneManagement.LoadSceneParameters parameters)
        {
            yield return LoadingScreen.ShowLoadingScreen();
            
            AsyncOperation ao = UnitySceneManager.LoadSceneAsync(sceneName, parameters);
            LoadingAsyncOperations.Add(ao);

            _lastLoadedScene = UnitySceneManager.GetSceneAt(UnitySceneManager.sceneCount - 1);

            CurrentAsyncOperation = ao;
            CurrentAsyncOperation.allowSceneActivation = scenesActivated;
            
            loadingScreenCoroutine = null;
        }

        /// <summary>
        /// Returns if all asynchronized tasks are considered IsDone.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator AsyncsIsDone()
        {
            bool notDone;
            do
            {
                notDone = loadingScreenCoroutine != null;
                foreach (AsyncOperation ao in LoadingAsyncOperations)
                {
                    if (!ao.isDone)
                    {
                        notDone = true;
                        break;
                    }
                }
                yield return null;
            } while (notDone);
        }
    }
}