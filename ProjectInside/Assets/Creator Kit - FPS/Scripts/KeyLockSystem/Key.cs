using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class Key : MonoBehaviour
{
    public string keyType;
    public Text KeyNameText;

    [Header("Audio")]
    public AudioClip KeyCollectAudioClip;
    [SerializeField] public EventReference KeyCollectEvent;

    void OnEnable()
    {
        KeyNameText.text = keyType;
    }

    void OnTriggerEnter(Collider other)
    {
        var keychain = other.GetComponent<Keychain>();

        if (keychain != null)
        {
            keychain.GrabbedKey(keyType);
            Destroy(gameObject);
        }
    }
}
