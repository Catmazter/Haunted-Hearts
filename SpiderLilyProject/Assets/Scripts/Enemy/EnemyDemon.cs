using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyDemon : EnemyBase
{

    [SerializeField] int runDistance;
   
  

    bool isRunningAway = false;
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



    private bool isRecoveringAfterRun = false;
    bool hasChosenRunDest = false;
    private bool isChasingPlayer = false;



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
        else if (isRecoveringAfterRun)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                roam();
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
        UpdateWarningSound();

    }
    protected override void roam()
    {
        isChasingPlayer = false;
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
        isChasingPlayer = true;
        base.chasePlayer();
      

    }

    private void runAway()
    {
        PlayScream();
        isChasingPlayer = false;
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
    }
    protected virtual void HandleRunningAway()
    {
        if (!hasChosenRunDest)
        {
            runAway(); 
            hasChosenRunDest = true;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {

            isRunningAway = false;
            hasChosenRunDest = false;
            isRecoveringAfterRun = true; 
            StartCoroutine(RunThenRoam());
        }
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

    void UpdateWarningSound()
    {
        if (warningSource == null || warningClip == null) return;

        float targetVolume = 0f;

        if (isChasingPlayer)
        {
            float distance = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

            if (distance <= maxHearDistance)
            {
                targetVolume = Mathf.Clamp01(1 - (distance / maxHearDistance));

                if (!isPlayingWarning)
                {
                    warningSource.clip = warningClip;
                    warningSource.loop = true;
                    warningSource.Play();
                    isPlayingWarning = true;
                }
            }
        }

        warningSource.volume = Mathf.Lerp(warningSource.volume, targetVolume, Time.deltaTime * fadeSpeed);

        if ((!isChasingPlayer || targetVolume <= 0.01f) && warningSource.volume <= 0.02f)
        {
            if (isPlayingWarning)
            {
                warningSource.Stop();
                isPlayingWarning = false;
            }
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

        roam(); 

        float roamTime = 10f;
        float timer = 0f;

        while (timer < roamTime)
        {
            timer += Time.deltaTime;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                roam();
            }

            yield return null;
        }
        isRecoveringAfterRun = false;

    }




}
