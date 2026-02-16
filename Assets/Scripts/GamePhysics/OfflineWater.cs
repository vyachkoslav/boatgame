using UnityEngine;
using Utility;

namespace GamePhysics
{
    public class OfflineWater : MonoBehaviour, IWater
    {
        [SerializeField] private Material waterMat;
        
        private int timeId;
        private int freqId;
        private int speedId;
        private int scaleId;

        private void Awake()
        {
            timeId = Shader.PropertyToID("_WaveTime");
            freqId = Shader.PropertyToID("_WaveFrequency");
            speedId = Shader.PropertyToID("_WaveSpeed");
            scaleId = Shader.PropertyToID("_WaveScale");
            GlobalObjects.Water = this;
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(GlobalObjects.Water, this))
                GlobalObjects.Water = null;
#if UNITY_EDITOR
            waterMat.SetFloat(timeId, 0);
#endif
        }

        private void Update()
        {
            waterMat.SetFloat(timeId, Time.time);
        }

        public float GetWaterPointHeight(Vector3 pos)
        {
            // in build those will be constant, so don't read from material
            var waveSpeed = waterMat.GetFloat(speedId);
            var waveScale = waterMat.GetFloat(scaleId);
            var waveFreq = waterMat.GetFloat(freqId);

            var waveLoc = waveSpeed * Time.time;
            var xPos = Mathf.Sin(pos.x * waveFreq + waveLoc);
            var zPos = Mathf.Cos(pos.z * waveFreq + waveLoc);
            var y = (xPos + zPos) * waveScale;
            return y;
        }
    }
}