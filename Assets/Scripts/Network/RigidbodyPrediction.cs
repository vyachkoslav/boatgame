using FishNet.Object.Prediction;
using FishNet.Serializing;
using FishNet.Transporting;
using FishNet.Utility.Template;
using UnityEngine;

namespace Network
{
    public class RigidbodyPrediction : TickNetworkBehaviour
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
            public ReconcileData(PredictionRigidbody pr)
            {
                Pr = pr;
                tick = 0;
            }

            public PredictionRigidbody Pr;

            private uint tick;

            public void Dispose() { }
            public uint GetTick() => tick;
            public void SetTick(uint value) => tick = value;
        }

        private readonly PredictionRigidbody pr = new();

        private void Awake()
        {
            pr.Initialize(GetComponent<Rigidbody>(), AutoPackType.Unpacked);
        }

        public override void OnStartNetwork()
        {
            SetTickCallbacks(TickCallback.PostTick);
        }

        protected override void TimeManager_OnPostTick()
        {
            PerformReplicate(default);
            CreateReconcile();
        }
        
        [Replicate]
        private void PerformReplicate(ReplicateData rd, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
        }
        
        /// <summary>
        /// Creates a reconcile that is sent to clients.
        /// </summary>
        public override void CreateReconcile()
        {
            ReconcileData rd = new(pr);
            PerformReconcile(rd);
        }
        
        [Reconcile]
        private void PerformReconcile(ReconcileData rd, Channel channel = Channel.Unreliable)
        {
            pr.Reconcile(rd.Pr);
        }
    }
}
