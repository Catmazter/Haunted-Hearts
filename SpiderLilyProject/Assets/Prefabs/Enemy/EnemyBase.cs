using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    //agent
    [SerializeField] NavMeshAgent agent;
    float stoppingDistOrig;
    [Space(2)]
    [Header("Roam")]
    float roamTimer;
    Vector3 startingPos;
    [SerializeField] float roamDist;
    [SerializeField] float roamPauseTimer;
    




    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        roam();
        
    }

    void roam()
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



}
