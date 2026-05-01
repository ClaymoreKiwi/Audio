using System;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Random = UnityEngine.Random;

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

    public Action<float, float> OnHealthChanged;

    private EventInstance m_LocalAudioInstance;              
    private static EventInstance s_GlobalDialogueInstance;

    void Awake()
    {
        Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("Target"));

        if(DestroyedEffect != null && PoolSystem.Instance != null)
        {
            PoolSystem.Instance.InitPool(DestroyedEffect, 16);
        }
    }

    void OnEnable()
    {
        m_CurrentHealth = health;
        m_Destroyed = false; 

        OnHealthChanged?.Invoke(m_CurrentHealth, health);

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
        OnHealthChanged?.Invoke(m_CurrentHealth, health);
        
        if(m_CurrentHealth > 0)
        {
            PlayHitAudio();
            return;
        }

        m_Destroyed = true; 

        // Stop and release the audio instance
        if (m_LocalAudioInstance.isValid())
        {
            m_LocalAudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            
            if (m_LocalAudioInstance.handle == s_GlobalDialogueInstance.handle)
            {
                s_GlobalDialogueInstance.clearHandle();
            }
            
            m_LocalAudioInstance.release();
            m_LocalAudioInstance.clearHandle();
        }
        
        if (p != null)
        {
            RuntimeManager.PlayOneShot(deathGrenadeEvent, transform.position);
        }
        else 
        {
            RuntimeManager.PlayOneShot(deathEvent, transform.position);
        }

        if (DestroyedEffect != null && PoolSystem.Instance != null)
        {
            var effect = PoolSystem.Instance.GetInstance<ParticleSystem>(DestroyedEffect);
            if (effect != null)
            {
                effect.time = 0.0f;
                effect.Play();
                effect.transform.position = transform.position;
            }
        }

        gameObject.SetActive(false);
        
        if (GameSystem.Instance != null)
        {
            GameSystem.Instance.TargetDestroyed(pointValue);
        }
    }

    private void PlayHitAudio()
    {
        if (m_LocalAudioInstance.isValid())
        {
            m_LocalAudioInstance.getPlaybackState(out PLAYBACK_STATE localState);
            if (localState != PLAYBACK_STATE.STOPPED)
            {
                return; 
            }
            
            // If the old sound is stopped, release it from memory before making a new one
            m_LocalAudioInstance.release();
            m_LocalAudioInstance.clearHandle();
        }

        bool wantsToSpeakDialogue = false;
        if (!hitDialogueEvent.IsNull)
        {
            if (Random.Range(0f, 100f) <= dialogueChance)
            {
                wantsToSpeakDialogue = true;
            }
        }

        if (wantsToSpeakDialogue && s_GlobalDialogueInstance.isValid())
        {
            s_GlobalDialogueInstance.getPlaybackState(out PLAYBACK_STATE globalState);
            if (globalState != PLAYBACK_STATE.STOPPED)
            {
                wantsToSpeakDialogue = false; 
            }
        }

        EventReference eventToPlay = wantsToSpeakDialogue ? hitDialogueEvent : hitVocalizationEvent;

        if (!eventToPlay.IsNull) 
        {
            m_LocalAudioInstance = RuntimeManager.CreateInstance(eventToPlay);
            m_LocalAudioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            m_LocalAudioInstance.start();
            
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
            m_LocalAudioInstance.release();
            m_LocalAudioInstance.clearHandle();
        }
    }
}