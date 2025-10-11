using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    //agent
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected int chaseSpeed;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] private Transform headPoint;
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
       
        agent.speed = chaseSpeed;

        Vector3 targetPos = gameManager.instance.player.transform.position;
        //Vector3 headOffset = headPoint.position - transform.position;
       
        agent.stoppingDistance = stoppingDistOrig;

        faceTarget();
        //agent.SetDestination(targetPos - headOffset);
        agent.SetDestination(gameManager.instance.player.transform.position);


    }
    void faceTarget( )
    {
        Vector3 direction = (gameManager.instance.player.transform.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * faceTargetSpeed);
        }
    }




}
