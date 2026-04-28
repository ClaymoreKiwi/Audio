using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Target : MonoBehaviour
{
    public float health = 5.0f;
    public int pointValue;

    public ParticleSystem DestroyedEffect;

    [Header("Audio")]
    [SerializeField] private EventReference deathEvent;
    [SerializeField] private EventReference deathGrenadeEvent;
    
    [Header("Hit Audio Setup")]
    [SerializeField] private EventReference hitDialogueEvent;      
    [SerializeField] private EventReference hitVocalizationEvent; 
    
    [Tooltip("Percentage chance (0 to 100) to play dialogue instead of a generic vocalization.")]
    [Range(0, 100)]
    [SerializeField] private float dialogueChance = 30f;          

    public RandomPlayer HitPlayer;
    public AudioSource IdleSource;
    
    public bool Destroyed => m_Destroyed;

    private bool m_Destroyed = false;
    private float m_CurrentHealth;

    // Tracks what this specific mob is currently saying/grunting
    private EventInstance m_LocalAudioInstance;              
    
    // Shared across all mobs to track if anyone is speaking Dialogue
    private static EventInstance s_GlobalDialogueInstance;

    void Awake()
    {
        Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("Target"));
        
        if(DestroyedEffect)
            PoolSystem.Instance.InitPool(DestroyedEffect, 16);
    }

    // Use OnEnable for initialization so pooled objects reset properly
    void OnEnable()
    {
        m_CurrentHealth = health;
        m_Destroyed = false; // Reset death state

        if(IdleSource != null && IdleSource.clip != null)
        {
            IdleSource.time = Random.Range(0.0f, IdleSource.clip.length);
            IdleSource.Play();
        }
    }

    public void Got(float damage, Projectile p = null)
    {
        if (m_Destroyed) return;

        m_CurrentHealth -= damage;
        
        // Survived the hit
        if(m_CurrentHealth > 0)
        {
            PlayHitAudio();
            return;
        }

        // Target is destroyed
        if (m_LocalAudioInstance.isValid())
        {
            // Stop any audio this mob is currently making since they are now dead
            m_LocalAudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            
            // If this instance was speaking dialogue and is now dead, clear the global dialogue instance so others can speak
            if (m_LocalAudioInstance.handle == s_GlobalDialogueInstance.handle)
            {
                s_GlobalDialogueInstance.clearHandle();
            }
        }
        
        // Play death sound
        if (p != null)
        {
            RuntimeManager.PlayOneShot(deathGrenadeEvent, transform.position);
        }
        else 
        {
            RuntimeManager.PlayOneShot(deathEvent, transform.position);
        }

        if (DestroyedEffect != null)
        {
            var effect = PoolSystem.Instance.GetInstance<ParticleSystem>(DestroyedEffect);
            effect.time = 0.0f;
            effect.Play();
            effect.transform.position = transform.position;
        }

        m_Destroyed = true;
        gameObject.SetActive(false);
        GameSystem.Instance.TargetDestroyed(pointValue);
    }

    private void PlayHitAudio()
    {
        // Check if this mob is already making a sound
        if (m_LocalAudioInstance.isValid())
        {
            m_LocalAudioInstance.getPlaybackState(out PLAYBACK_STATE localState);
            if (localState != PLAYBACK_STATE.STOPPED)
            {
                return; // Let them finish what they are saying/grunting
            }
        }

        // Decide if they want to speak dialogue
        bool wantsToSpeakDialogue = false;
        if (!hitDialogueEvent.IsNull)
        {
            if (Random.Range(0f, 100f) <= dialogueChance)
            {
                wantsToSpeakDialogue = true;
            }
        }

        // If they want to speak dialogue, check if anyone is currently speaking dialogue globally
        if (wantsToSpeakDialogue && s_GlobalDialogueInstance.isValid())
        {
            s_GlobalDialogueInstance.getPlaybackState(out PLAYBACK_STATE globalState);
            if (globalState != PLAYBACK_STATE.STOPPED)
            {
                wantsToSpeakDialogue = false; // Fallback to grunt
            }
        }

        // Assign the final event based on the logic above
        EventReference eventToPlay = wantsToSpeakDialogue ? hitDialogueEvent : hitVocalizationEvent;

        if (!eventToPlay.IsNull) 
        {
            m_LocalAudioInstance = RuntimeManager.CreateInstance(eventToPlay);
            m_LocalAudioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            m_LocalAudioInstance.start();
            
            // Release immediately since we won't need to manipulate this instance again
            m_LocalAudioInstance.release(); 
            
            if (wantsToSpeakDialogue)
            {
                s_GlobalDialogueInstance = m_LocalAudioInstance;
            }
        }
    }

    private void OnDestroy()
    {
        if (m_LocalAudioInstance.isValid())
        {
            m_LocalAudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}