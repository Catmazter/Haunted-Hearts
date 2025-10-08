using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    //agent
    [SerializeField] protected NavMeshAgent agent;
    float stoppingDistOrig;
    [Space(2)]
    [Header("Roam")]
    protected float roamTimer;
    Vector3 startingPos;
    [SerializeField] protected float roamDist;
    [SerializeField] protected float roamPauseTimer;
    




    protected virtual void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;
        // roam();

    }

    protected virtual void roam()
    {
        if (roamTimer >= roamPauseTimer && agent.remainingDistance < 0.01f)
        {

            roamTimer = 0;
            agent.stoppingDistance = 0;

            Vector3 ranPos = Random.insideUnitSphere * roamDist;
            ranPos += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);

            agent.SetDestination(hit.position);
        }
    }

    protected virtual void chasePlayer()
    {

    }



}
