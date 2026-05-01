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
    [SerializeField] private EventReference jumpEvent;
    [SerializeField] private EventReference jumpLandEventVocals;
    [SerializeField] private EventReference jumpLandEvent;
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
        
        PlayClip(GetRandomClip(), PitchMin, PitchMax, false, false);
    }

    public void PlayClip(AudioClip clip, float pitchMin, float pitchMax, bool ComingFromJump, bool isLanding)
    {
        EventInstance instance;
        EventInstance instance2 = RuntimeManager.CreateInstance(jumpLandEventVocals);
        if(!ComingFromJump && !isLanding)
        {
            instance = RuntimeManager.CreateInstance(footstepEvent);
        }
        else if(isLanding)
        {
            // this will be our landing
            instance = RuntimeManager.CreateInstance(jumpLandEvent);
            instance2.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            RaycastChecker(instance2);
            instance2.start();
            instance2.release();
        }
        else
        {
            instance = RuntimeManager.CreateInstance(jumpEvent);
        }

        RaycastChecker(instance);

        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

        float randomPitch = Random.Range(pitchMin, pitchMax);
        instance.setParameterByName("PitchShifterVar", randomPitch);

        instance.start();
        instance.release();
        //RuntimeManager.PlayOneShot(footstepEvent, transform.position);
    }

    void RaycastChecker(EventInstance instance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            //we have tags for each room, just need these to pass into fmod for the correct sound
            instance.setParameterByNameWithLabel("RoomSounds", hit.collider.tag);
        }
    }
}
