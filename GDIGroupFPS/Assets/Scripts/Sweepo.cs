using UnityEngine;
using UnityEngine.AI;

public class Sweepo : MonoBehaviour
{
    public float roamRadius = 10f;
    public float roamTimer = 5f;

    private NavMeshAgent agent;
    private float timer;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = roamTimer; 
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= roamTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, roamRadius, -1);
            agent.SetDestination(newPos);
            timer = 0; 
        }
    }


    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}