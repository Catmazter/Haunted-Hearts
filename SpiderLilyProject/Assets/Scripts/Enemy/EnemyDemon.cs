using UnityEngine;
using UnityEngine.AI;

public class EnemyDemon : EnemyBase
{
    [SerializeField] int runDistance;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //roam();
        //chasePlayer();
        runAway();
    }

    private void runAway()
    {
        Vector3 oppositeDir = -transform.forward;
        Vector3 targetPos = transform.position + oppositeDir * runDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, runDistance, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.stoppingDistance = 0;
            agent.SetDestination(hit.position);
            transform.rotation = Quaternion.LookRotation(oppositeDir);

            Debug.Log(" Enemy is running away to: " + hit.position);
        }
        else
        {
            Vector3 randomDir = Random.insideUnitSphere * runDistance + transform.position;
            if (NavMesh.SamplePosition(randomDir, out hit, runDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                transform.rotation = Quaternion.LookRotation(hit.position - transform.position);
                Debug.Log("Enemy ran away using random direction (fallback).");
            }
            else
            {
                Debug.LogWarning("No valid NavMesh position found for run away.");
            }
        }
    }
}