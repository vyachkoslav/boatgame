using System;
using FishNet.Object.Prediction;
using FishNet.Serializing;
using FishNet.Transporting;
using FishNet.Utility.Template;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

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
        [SerializeField] private float waterDrag = 20f;
        
        [SerializeField] private float maxPidTorque;
        [SerializeField] private float pFactor;
        
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
        private void PerformReplicate(ReplicateData rd, 
            ReplicateState state = ReplicateState.Invalid,
            Channel channel = Channel.Unreliable)
        {
            if (state.IsFuture())
                return;

            // keeping z rotation the same and y in bounds
            var rot = paddleRb.transform.localEulerAngles;
            rot.y = YToBounds(rd.State, rot.y, out var wasLower, out var wasHigher);
            rot.z = zRot;
            paddle.MoveRotation(paddleRb.transform.parent.rotation * Quaternion.Euler(rot));
            
            // if hit bounds, reset to bound and stop
            var calculateDeltaForce = true;
            if (wasLower || wasHigher)
            {
                var angVel = paddleRb.transform.InverseTransformDirection(paddleRb.angularVelocity);
                angVel.y = 0;
                angVel = paddleRb.transform.TransformDirection(angVel);
                paddle.AngularVelocity(angVel);
                var delta = isLeft ? rd.Delta : -rd.Delta;
                // if force directed into bound, don't calculate force from mouse delta
                if (wasLower && delta <= 0 || wasHigher && delta >= 0)
                    calculateDeltaForce = false;
            }

            if (calculateDeltaForce)
            {
                // player mouse rotation
                var mDelta = Mathf.Clamp((float)(rd.Delta / TimeManager.TickDelta), -maxDelta, maxDelta);
                var vel = new Vector3(0, mDelta, 0);
                paddle.AddRelativeTorque(vel, ForceMode.Force);
            }
            
            // paddle up/down
            var xRot = rd.State switch
            {
                PaddleState.Middle => 0f,
                PaddleState.Down => -45f,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (!state.IsTickedNonCreated())
            {
                var xTorq = Mathf.DeltaAngle(rot.x, xRot) * pFactor;
                xTorq = Mathf.Clamp(xTorq, -maxPidTorque, maxPidTorque);
                xTorq *= isLeft ? 1 : -1;
                paddle.AddRelativeTorque(new Vector3(xTorq, 0, 0));
            }
            
            // water drag
            var bladePos = blade.transform.position;
            var waterLevel = GlobalObjects.Water.GetWaterPointHeight(bladePos);
            if (bladePos.y < waterLevel)
            {
                var vel = paddleRb.GetPointVelocity(bladePos);
                var force = Vector3.Project(-vel * waterDrag, blade.forward);
                paddle.AddForceAtPosition(force, bladePos);
            }

            paddle.Simulate();
        }

        private float YToBounds(PaddleState state, float y, out bool wasLower, out bool wasHigher)
        {
            wasLower = false;
            wasHigher = false;
            if (state == PaddleState.Down)
            {
                if (y < lowerMinAngle)
                {
                    y = lowerMinAngle;
                    wasLower = true;
                }
                else if (y > lowerMaxAngle)
                {
                    y = lowerMaxAngle;
                    wasHigher = true;
                }
            }

            return y;
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