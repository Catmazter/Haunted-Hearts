using UnityEngine;
using UnityEngine.AI;

public class EnemyDemon : EnemyBase
{
    
    [SerializeField] int runDistance;
    [SerializeField] float stunDuration;
    
    bool isRunningAway = false;
    bool isStunned;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource; 
    [SerializeField] AudioClip screamClip;
    float stunTimer;
    bool hasChosenRunDest = false;

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
        if (isRunningAway)
        {
            HandleRunningAway();
        }
        else
        {
            chasePlayer();
        }
        UpdateAnimation();
    }
    protected override void chasePlayer()
    {

        agent.speed = chaseSpeed;
        base.chasePlayer();

    }

    private void runAway()
    {
        PlayScream();
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
    protected virtual void HandleRunningAway()
    {
        if (isStunned)
        {
            stunTimer += Time.deltaTime;
            if (stunTimer >= stunDuration)
            {
                isStunned = false;
                isRunningAway = false; 
                agent.isStopped = false;
                hasChosenRunDest = false;
            }
            return;
        }
        if (!hasChosenRunDest)
        {
            int choice = Random.Range(0, 2);
            if (choice == 0)
            {
                stunt();
            }
            else
            {
                runAway();
            }
            hasChosenRunDest = true;
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isRunningAway = false;
            hasChosenRunDest = false;
        }
    }

    private void stunt()
    {
        PlayScream();
        isStunned = true;
        stunTimer = 0f;
        agent.isStopped = true;
        Debug.Log("Enemy stunned!");
    }

    void UpdateAnimation()
    {
        
        if (agent.velocity.magnitude > 0.1f)
        {
            anim.speed = 1.5f; 
        }
        else
        {
            anim.speed = 0f; 
        }
    }
    void PlayScream()
    {
        if (audioSource != null && screamClip != null)
        {
            audioSource.PlayOneShot(screamClip);
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Match"))
        {
            Debug.Log("Demon hit by a match!");
            OnHitByMatch();
        }
    }

    void OnHitByMatch()
    {
        if (!isRunningAway)
        {
            isRunningAway = true;
            hasChosenRunDest = false;
            PlayScream();
        }
    }

}