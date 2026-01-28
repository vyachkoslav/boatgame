using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Network
{
    public abstract class NetworkPuzzle : NetworkBehaviour
    {
        private readonly SyncVar<bool> isPuzzleStarted = new();
        private readonly SyncVar<bool> isPuzzleEnded = new();
        
        [Server]
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!isPuzzleStarted.Value && other.CompareTag("Boat"))
                StartPuzzle();
        }

        [Server]
        protected void StartPuzzle()
        {
            if (isPuzzleStarted.Value || isPuzzleEnded.Value)
            {
                Debug.LogError("Trying to start a puzzle second time");
                return;
            }
            
            isPuzzleStarted.Value = true;
            OnPuzzleStart();
        }

        [Server]
        protected void EndPuzzle()
        {
            if (!isPuzzleStarted.Value || isPuzzleEnded.Value)
            {
                Debug.LogError("Trying to end a non-started puzzle or already finished one");
                return;
            }
            
            isPuzzleEnded.Value = true;
            OnPuzzleEnd();
        }

        public override void OnStartClient()
        {
            // Server handles them separately
            if (IsServerStarted) return;
            
            if (isPuzzleEnded.Value)
                OnPuzzleEnd();
            else if (isPuzzleStarted.Value)
                OnPuzzleStart();
            
            // shouldn't be set to false
            isPuzzleStarted.OnChange += (_, __, ___) => OnPuzzleStart();
            isPuzzleEnded.OnChange += (_, __, ___) => OnPuzzleEnd();
        }

        protected abstract void OnPuzzleStart();
        protected abstract void OnPuzzleEnd();
    }
}