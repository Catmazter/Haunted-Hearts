using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    //agent
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected int chaseSpeed;
    [SerializeField] int faceTargetSpeed;
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
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            faceTarget();
        }
            agent.stoppingDistance = stoppingDistOrig;
            agent.SetDestination(gameManager.instance.player.transform.position);

    }
    void faceTarget( )
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(gameManager.instance.player.transform.position.x, transform.position.y, gameManager.instance.player.transform.position.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }




}
