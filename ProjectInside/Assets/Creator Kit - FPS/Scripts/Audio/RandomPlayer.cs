using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomPlayer : MonoBehaviour
{
    public AudioClip[] Clips;
    public float PitchMin = 1.0f;
    public float PitchMax = 1.0f;
    [SerializeField] private EventReference footstepEvent;
    public string[] roomTags;

    void Awake()
    {
        roomTags = UnityEditorInternal.InternalEditorUtility.tags;
    }

    public AudioClip GetRandomClip()
    {
        return Clips[Random.Range(0, Clips.Length)];
    }

    public void PlayRandom()
    {
        if(Clips.Length == 0)
            return;
        
        PlayClip(GetRandomClip(), PitchMin, PitchMax);
    }

    public void PlayClip(AudioClip clip, float pitchMin, float pitchMax)
    {
        EventInstance instance = RuntimeManager.CreateInstance(footstepEvent);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            //we have tags for each room, just need these to pass into fmod for the correct sound
            instance.setParameterByNameWithLabel("RoomSounds", hit.collider.tag);
        }

        float randomPitch = Random.Range(pitchMin, pitchMax);
        instance.setParameterByName("PitchShifterVar", randomPitch);

        instance.start();
        instance.release();
        //RuntimeManager.PlayOneShot(footstepEvent, transform.position);
    }
}
