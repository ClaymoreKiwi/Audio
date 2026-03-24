using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class UIAudioPlayer : MonoBehaviour
{
    public static UIAudioPlayer Instance { get; private set; }

    public AudioClip PositiveSound;
    public AudioClip NegativeSound;
   
    
    AudioSource m_Source;

    void Awake()
    {
        m_Source = GetComponent<AudioSource>();
        Instance = this;
    }

    public static void PlayPositive()
    {
       // Instance.m_Source.PlayOneShot(Instance.PositiveSound);
        RuntimeManager.PlayOneShot("event:/UI Enter", Vector3.zero);
    }

    public static void PlayNegative()
    {
        //Instance.m_Source.PlayOneShot(Instance.NegativeSound);
        RuntimeManager.PlayOneShot("event:/UI Exit", Vector3.zero);
    }
}
