using FMODUnity;
using UnityEngine;public class EnemyDeath : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private EventReference deathEvent;

    public void Die()
    {
        RuntimeManager.PlayOneShot(deathEvent, transform.position);
        Destroy(gameObject);
    }
}
