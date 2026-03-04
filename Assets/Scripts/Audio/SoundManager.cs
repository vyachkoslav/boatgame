using System;
using FishNet.Object;
using UnityEngine;

public class SoundManager : NetworkBehaviour
{
    private static SoundManager _instance;

    public static SoundManager Instance { get { return _instance; } }

    private static float sfxvolume = 1;
    public float SFXvolume { get { return sfxvolume; } set { sfxvolume = value; } }


    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxvolume);
        }
    }


    //Used for  3D sounds might not be used in this game
    /*public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }*/

    //Used for 2D sounds, like menu sounds
    [ObserversRpc]
    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName), sfxvolume);
    }

        public void UpdateSFXVolume(float value)
    {
        sfxvolume = value;
    }


}
