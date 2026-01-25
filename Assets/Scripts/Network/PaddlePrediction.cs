using System;
using FishNet.Object.Prediction;
using FishNet.Serializing;
using FishNet.Transporting;
using FishNet.Utility.Template;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Network
{
    public class PaddlePrediction : TickNetworkBehaviour
    {
        #region Types.

        private enum PaddleState
        {
            Middle,
            Down
        }
        
        private struct ReplicateData : IReplicateData
        {
            public ReplicateData(float delta, PaddleState state)
            {
                State = state;
                Delta = delta;
                tick = 0;
            }

            /// <summary>
            /// Which state is paddle in
            /// </summary>
            public PaddleState State;
            
            /// <summary>
            /// Mouse movement to move paddle
            /// </summary>
            public float Delta;

            private uint tick;

            public void Dispose() { }
            public uint GetTick() => tick;
            public void SetTick(uint value) => tick = value;
        }

        private struct ReconcileData : IReconcileData
        {
            public ReconcileData(PredictionRigidbody paddle)
            {
                Paddle = paddle;
                tick = 0;
            }

            public PredictionRigidbody Paddle;

            private uint tick;

            public void Dispose() { }
            public uint GetTick() => tick;
            public void SetTick(uint value) => tick = value;
        }

        #endregion

        [SerializeField] private InputActionReference lowerPaddleAction;
        
        [SerializeField] private Rigidbody paddleRb;
        [SerializeField] private Transform blade;
        [SerializeField] private float lowerMinAngle;
        [SerializeField] private float lowerMaxAngle;
        [SerializeField] private bool isLeft;

        [SerializeField] private float mouseSensitivity = 0.01f;
        [SerializeField] private float maxDelta = 100;
        
        private readonly PredictionRigidbody paddle = new();
        private PaddleState currentState = PaddleState.Middle;
        private float zRot;
        
        private float deltaPending;

        private void Awake()
        {
            zRot = paddleRb.transform.localRotation.eulerAngles.z;
            paddle.Initialize(paddleRb, AutoPackType.Unpacked);
        }

        private void OnEnable()
        {
            lowerPaddleAction.action.started += LowerPaddle;
            lowerPaddleAction.action.canceled += RaisePaddle;
        }
        
        private void OnDisable()
        {
            lowerPaddleAction.action.started -= LowerPaddle;
            lowerPaddleAction.action.canceled -= RaisePaddle;
        }

        private void Update()
        {
            deltaPending += Pointer.current.delta.value.y;
        }

        private void LowerPaddle(InputAction.CallbackContext callbackContext)
        {
            if (!IsOwner) return;
            var angle = paddleRb.transform.localEulerAngles.y;
            if (angle < lowerMinAngle || angle > lowerMaxAngle) return;
            
            currentState = PaddleState.Down;
        }
        
        private void RaisePaddle(InputAction.CallbackContext callbackContext)
        {
            if (!IsOwner) return;
            currentState = PaddleState.Middle;
        }
        
        public override void OnStartNetwork()
        {
            // Rigidbodies need tick and postTick.
            SetTickCallbacks(TickCallback.Tick | TickCallback.PostTick);
        }

        protected override void TimeManager_OnTick()
        {
            PerformReplicate(BuildMoveData());
        }

        protected override void TimeManager_OnPostTick()
        {
            CreateReconcile();
        }

        /// <summary>
        /// Returns replicate data to send as the controller.
        /// </summary>
        private ReplicateData BuildMoveData()
        {
            if (!IsOwner) return default;

            ReplicateData md = new(-deltaPending * mouseSensitivity, currentState);
            deltaPending = 0;
            return md;
        }
        
        [Replicate]
        private void PerformReplicate(ReplicateData rd, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            if (state.IsFuture())
                return;
            
            var xRot = rd.State switch
            {
                PaddleState.Middle => 0,
                PaddleState.Down => -45,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var rot = paddleRb.transform.localEulerAngles;
            if (!state.IsTickedNonCreated())
                rot.x = xRot;
            
            var wasHigher = false;
            var wasLower = false;
            if (rd.State == PaddleState.Down)
            {
                if (rot.y < lowerMinAngle)
                {
                    rot.y = lowerMinAngle+1;
                    wasLower = true;
                }
                else if (rot.y > lowerMaxAngle)
                {
                    rot.y = lowerMaxAngle-1;
                    wasHigher = true;
                }
            }
            rot.z = zRot;
            paddle.MoveRotation(paddleRb.transform.parent.rotation * Quaternion.Euler(rot));
            // if hit bounds, reset to bound and stop
            if (wasLower || wasHigher)
            {
                paddle.AngularVelocity(Vector3.zero);
                var delta = isLeft ? rd.Delta : -rd.Delta;
                // if force directed into bound, return
                if (wasLower && delta <= 0 || wasHigher && delta >= 0)
                {
                    paddle.Simulate();
                    return;
                }
            }

            var mdelta = Mathf.Clamp((float)(rd.Delta / TimeManager.TickDelta), -maxDelta, maxDelta);
            var vel = new Vector3(0, mdelta, 0);
            paddle.AddRelativeTorque(vel, ForceMode.Force);
            
            paddle.Simulate();
        }
        
        /// <summary>
        /// Creates a reconcile that is sent to clients.
        /// </summary>
        public override void CreateReconcile()
        {
            ReconcileData rd = new(paddle);
            PerformReconcile(rd);
        }
        
        [Reconcile]
        private void PerformReconcile(ReconcileData rd, Channel channel = Channel.Unreliable)
        {
            paddle.Reconcile(rd.Paddle);
        }
    }
}