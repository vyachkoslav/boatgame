using System;
using UnityEngine;
using Utility;

public class Water : MonoBehaviour
{
    [SerializeField] private Material waterMat;
    private int freqId;
    private int speedId;
    private int scaleId;

    private void Awake()
    {
        GlobalObjects.Water = this;
        freqId = Shader.PropertyToID("_WaveFrequency");
        speedId = Shader.PropertyToID("_WaveSpeed");
        scaleId = Shader.PropertyToID("_WaveScale");
    }

    public float GetWaterPointHeight(Vector3 pos)
    {
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
