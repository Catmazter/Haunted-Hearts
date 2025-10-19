using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyDemon : EnemyBase
{

    [SerializeField] int runDistance;
    [SerializeField] float stunDuration;
  

    bool isRunningAway = false;
    bool isStunned;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip screamClip;
    [Range(0f, 1f)] public float demonVolume = 0.5f;
    [SerializeField] private AudioSource warningSource;
    [SerializeField] private AudioClip warningClip;
    [SerializeField] private float maxHearDistance = 15f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] float sightRange = 20f;
    [SerializeField] float sightAngle = 120f;
    private bool isPlayingWarning = false;
    public bool onMesh;



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
            if (isPlayerOnNavMesh())
            {
                chasePlayer();
            }
            else
            {
                roam();

            }
        }
        onMesh = isPlayerOnNavMesh();
        //Debug.Log($"[EnemyDemon] Player on NavMesh: {onMesh} | Agent stopped: {agent.isStopped} | Agent pathPending: {agent.pathPending}");
        UpdateAnimation();

    }
    protected override void roam()
    {
        base.roam();
        if (CanSeePlayer())
        {
          //  Debug.Log("Demon spotted the player while roaming!");
            chasePlayer();
        }
    }
    bool CanSeePlayer()
    {
        if (gameManager.instance == null || gameManager.instance.player == null) return false;

        Vector3 directionToPlayer = (gameManager.instance.player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
        
        if (distanceToPlayer > sightRange) return false;
        
        if (Vector3.Angle(transform.forward, directionToPlayer) > sightAngle / 2f) return false;

        // Ray
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, directionToPlayer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, sightRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    protected override void chasePlayer()
    {

        base.chasePlayer();
        if (warningSource == null || warningClip == null) return;

        float targetVolume = 0f;

        if (agent.remainingDistance <= maxHearDistance)
        {
            targetVolume = Mathf.Clamp01(1 - (agent.remainingDistance / maxHearDistance));
            if (!isPlayingWarning)
            {
                warningSource.clip = warningClip;
                warningSource.loop = true;
                warningSource.Play();
                isPlayingWarning = true;
            }
        }
        else
        {
            targetVolume = 0f;
            if (isPlayingWarning && warningSource.volume <= 0.01f)
            {
                warningSource.Stop();
                isPlayingWarning = false;
            }
        }

        warningSource.volume = Mathf.Lerp(warningSource.volume, targetVolume, Time.deltaTime * fadeSpeed);


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

           // Debug.Log(" Enemy is running away to: " + hit.position);
        }
        else
        {
            Vector3 randomDir = Random.insideUnitSphere * runDistance + transform.position;
            if (NavMesh.SamplePosition(randomDir, out hit, runDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                transform.rotation = Quaternion.LookRotation(hit.position - transform.position);
              //  Debug.Log("Enemy ran away using random direction (fallback).");
            }
            else
            {
             //   Debug.LogWarning("No valid NavMesh position found for run away.");
            }
        }
        StartCoroutine(RunThenRoam());
    }
    protected virtual void HandleRunningAway()
    {
        if (isStunned)
        {
            stunTimer += Time.deltaTime;
            agent.speed = Mathf.Lerp(agent.speed, 0f, Time.deltaTime * 2f);

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
        StartCoroutine(RunThenRoam());
       // Debug.Log("Enemy stunned!");
    }

    void UpdateAnimation()
    {

        if (agent.velocity.magnitude > 0.1f)
        {
            anim.speed = 1.5f;
        }
        else
        {
            anim.speed = Mathf.Lerp(anim.speed, 0, Time.deltaTime * 3);

        }
    }
    void PlayScream()
    {
        if (audioSource != null && screamClip != null)
        {
            audioSource.PlayOneShot(screamClip, demonVolume);
        }
    }




    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Match"))
        {
           // Debug.Log("Demon hit by a match!");
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

    bool isPlayerOnNavMesh()
    {
        NavMeshHit hit;

        return NavMesh.SamplePosition(gameManager.instance.player.transform.position, out hit, 3.0f, NavMesh.AllAreas);
    }

    private IEnumerator RunThenRoam()
    {
        // Wait until demon reaches the run destination
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
        roam();
        yield return new WaitForSeconds(15); 
    }




}
