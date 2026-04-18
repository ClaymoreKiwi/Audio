using UnityEngine;
using UnityEngine.AI;

public class followPlayer : MonoBehaviour
{
    public Transform target;
    public float updateRate = 0.2f; // how often to refresh destination

    private NavMeshAgent agent;
    private float timer;

    void Awake()
    {
        target = GameObject.Find("Character").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null || agent == null)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= updateRate)
        {
            timer = 0f;
            agent.SetDestination(target.position);
        }
    }
}
