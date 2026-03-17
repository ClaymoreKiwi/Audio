using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Collections;

public class Heartbeat : MonoBehaviour
{
    [SerializeField] private EventReference heartDistance;
    [SerializeField] private GameObject Player;
    EventInstance instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = RuntimeManager.CreateInstance(heartDistance);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        instance.start();
        //instance.Release();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (transform.position - Player.transform.position).sqrMagnitude;
        instance.setParameterByName("DistanceHeart", distance);
    }
}
