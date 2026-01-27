using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Utility;

namespace GamePhysics
{
    public class Water : NetworkBehaviour, ISerializationCallbackReceiver
    {
        private readonly SyncVar<DateTime> timeStarted = new();
    
        [SerializeField] private Material waterMat;
        
        [HideInInspector] [SerializeField] private float sFreq;
        [HideInInspector] [SerializeField] private float sSpeed;
        [HideInInspector] [SerializeField] private float sScale;
        
        private int freqId;
        private int speedId;
        private int scaleId;
        
        private int timeOffsetId;

        private float timeOffset;
        
        private void Awake()
        {
            GlobalObjects.Water = this;
#if UNITY_EDITOR
            freqId = Shader.PropertyToID("_WaveFrequency");
            speedId = Shader.PropertyToID("_WaveSpeed");
            scaleId = Shader.PropertyToID("_WaveScale");
#endif
        }

        public override void OnStartServer()
        {
            timeStarted.Value = DateTime.Now;
        }

        public override void OnStartClient()
        {
            timeOffset = (float)(DateTime.Now - timeStarted.Value).TotalSeconds - Time.timeSinceLevelLoad;
            waterMat.SetFloat("_TimeOffset", timeOffset);
        }

        public float GetWaterPointHeight(Vector3 pos)
        {
            // in build those will be constant, so don't read from material
#if UNITY_EDITOR
            var waveSpeed = waterMat.GetFloat(speedId);
            var waveScale = waterMat.GetFloat(scaleId);
            var waveFreq = waterMat.GetFloat(freqId);
#else
            var waveSpeed = sSpeed;
            var waveScale = sScale;
            var waveFreq = sFreq;
#endif
        
            var waveLoc = waveSpeed * (Time.timeSinceLevelLoad + timeOffset);
            var xPos = Mathf.Sin(pos.x * waveFreq + waveLoc);
            var zPos = Mathf.Cos(pos.z * waveFreq + waveLoc);
            var y = (xPos + zPos) * waveScale;
            return y;
        }

        public void OnBeforeSerialize()
        {
            sSpeed = waterMat.GetFloat("_WaveSpeed");
            sScale = waterMat.GetFloat("_WaveScale");
            sFreq = waterMat.GetFloat("_WaveFrequency");
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
