using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Utility;

namespace Network
{
    public abstract class NetworkPuzzle : NetworkBehaviour
    {
        [Serializable]
        public enum State
        {
            None,
            Started,
            Failed,
            FailedOnTime,
            Success,
        }

        private readonly SyncVar<State> state = new();
        private readonly SyncTimer timer = new();

        [SerializeField] private float timeToSolve;
        [SerializeField] private string puzzleInstructions;
        
        [Server]
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (state.Value == State.None && other.CompareTag("Boat"))
                StartPuzzle();
        }

        [Server]
        protected void StartPuzzle()
        {
            if (state.Value != State.None)
            {
                Debug.LogWarning("Trying to start a puzzle second time");
                return;
            }
            
            state.Value = State.Started;
            timer.StartTimer(timeToSolve);
        }

        [Server]
        protected void EndPuzzle(State result)
        {
            if (state.Value != State.Started)
            {
                Debug.LogWarning("Trying to end a non-started puzzle or already finished one");
                return;
            }
            
            timer.StopTimer();
            state.Value = result;
        }

        private void TimerOnChange(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            if (op != SyncTimerOperation.Finished) return;
            
            state.Value = State.FailedOnTime;
        }

        public override void OnStartNetwork()
        {
            state.OnChange += StateOnChange;
        }

        public override void OnStartServer()
        {
            timer.OnChange += TimerOnChange;
        }

        public override void OnStartClient()
        {
            StateOnChange(default, state.Value, false);
        }

        private void StateOnChange(State prev, State next, bool asServer)
        {
            if (IsHostStarted && asServer) return;
            
            switch (next)
            {
                case State.None: break;
                case State.Started:
                    GlobalObjects.PuzzleHUD.ShowTimer();
                    GlobalObjects.PuzzleHUD.ShowNotification(puzzleInstructions);
                    OnPuzzleStart();
                    break;
                default:
                    GlobalObjects.PuzzleHUD.HideTimer();
                    GlobalObjects.PuzzleHUD.ShowNotification(next == State.Success ? "Done!" : "Failed :(");
                    OnPuzzleEnd(next);
                    break;
            }
        }

        protected virtual void Update()
        {
            if (timer.Paused) return;
            
            timer.Update();
            GlobalObjects.PuzzleHUD.UpdateTimer(timer.Remaining);
        }

        protected abstract void OnPuzzleStart();
        protected abstract void OnPuzzleEnd(State result);
    }
}