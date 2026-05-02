using UnityEngine;
using FMODUnity;

public class BossMusicController : MonoBehaviour
{
    public Target bossTarget;
    public StudioEventEmitter bossMusicEmitter;

    private int m_CurrentMusicStage = 0;

    private void OnEnable()
    {
        if (bossTarget != null)
        {
            bossTarget.OnHealthChanged += UpdateBossMusicStage;
        }
    }

    private void OnDisable()
    {
        if (bossTarget != null)
        {
            bossTarget.OnHealthChanged -= UpdateBossMusicStage;
        }
    }

    private void UpdateBossMusicStage(float currentHealth, float maxHealth)
    {
        if (bossMusicEmitter == null) return;

        float healthPercentage = currentHealth / maxHealth;
        int newStage = 0;

        if (currentHealth <= 0)
        {
            newStage = 3; // Boss defeated stage
        }
        else if (healthPercentage <= 0.33f) 
        {
            newStage = 2;
        }
        else if (healthPercentage <= 0.66f) 
        {
            newStage = 1;
        }

        // --- DEBUG LOGS ADDED HERE ---
        Debug.Log($"Boss Health: {currentHealth}/{maxHealth} ({healthPercentage * 100}%). Calculated Stage: {newStage}");

        if (newStage != m_CurrentMusicStage)
        {
            Debug.Log($"---> SENDING TO FMOD: Changing BossStageStepper from {m_CurrentMusicStage} to {newStage}");
            m_CurrentMusicStage = newStage;
            bossMusicEmitter.SetParameter("BossStageStepper", m_CurrentMusicStage);
        }
    }
}