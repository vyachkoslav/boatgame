using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;
using Utility;

namespace GamePhysics
{
    public class OfflineWater : MonoBehaviour
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

        }


#if UNITY_EDITOR
        private void OnDestroy()
        {
            waterMat.SetFloat("_WaveTime", 0);
        }
#endif




        private void Update()
        {
            waterMat.SetFloat(timeId, Time.time);
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
        
            var waveLoc = waveSpeed * Time.time;
            var xPos = Mathf.Sin(pos.x * waveFreq + waveLoc);
            var zPos = Mathf.Cos(pos.z * waveFreq + waveLoc);
            var y = (xPos + zPos) * waveScale;
            return y;
        }


    }
}
