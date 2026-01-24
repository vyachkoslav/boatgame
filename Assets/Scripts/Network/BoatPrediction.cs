using FishNet.Object.Prediction;
using FishNet.Transporting;
using FishNet.Utility.Template;
using UnityEngine;

namespace Network
{
    public class BoatPrediction : TickNetworkBehaviour
    {
        private struct ReplicateData : IReplicateData
        {
            private uint tick;

            public void Dispose() { }
            public uint GetTick() => tick;
            public void SetTick(uint value) => tick = value;
        }
        
        private struct ReconcileData : IReconcileData
        {
            public ReconcileData(PredictionRigidbody boat)
            {
                Boat = boat;
                tick = 0;
            }

            public PredictionRigidbody Boat;

            private uint tick;

            public void Dispose() { }
            public uint GetTick() => tick;
            public void SetTick(uint value) => tick = value;
        }

        [SerializeField] private Rigidbody boatRb;
        [SerializeField] private Transform rBlade;
        [SerializeField] private Transform lBlade;
        [SerializeField] private float waterDrag;

        private readonly PredictionRigidbody boat = new();
        private Vector3 rLast, lLast;

        private void Awake()
        {
            boat.Initialize(boatRb);
            rLast = rBlade.position;
            lLast = lBlade.position;
        }

        public override void OnStartNetwork()
        {
            // Rigidbodies need tick and postTick.
            SetTickCallbacks(TickCallback.Tick | TickCallback.PostTick);
        }

        protected override void TimeManager_OnTick()
        {
            PerformReplicate(default);
            CreateReconcile();
        }
        
        [Replicate]
        private void PerformReplicate(ReplicateData rd, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            var cur = rBlade.transform.position;
            var last = rLast;
            if (cur.y < 0 && last.y < 0) 
            {
                var velocity = (cur - last) / Time.fixedDeltaTime;
                var force = -velocity * waterDrag;
                boat.AddForceAtPosition(force, cur, ForceMode.Force);
            }
            rLast = cur;
            
            cur = lBlade.transform.position;
            last = lLast;
            if (cur.y < 0 && last.y < 0) 
            {
                var velocity = (cur - last) / Time.fixedDeltaTime;
                var force = -velocity * waterDrag;
                boat.AddForceAtPosition(force, cur, ForceMode.Force);
            }
            lLast = cur;
            
            boat.Simulate();
        }
        
        /// <summary>
        /// Creates a reconcile that is sent to clients.
        /// </summary>
        public override void CreateReconcile()
        {
            ReconcileData rd = new(boat);
            PerformReconcile(rd);
        }
        
        [Reconcile]
        private void PerformReconcile(ReconcileData rd, Channel channel = Channel.Unreliable)
        {
            boat.Reconcile(rd.Boat);
        }
    }
}
