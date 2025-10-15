using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyDemon : EnemyBase
{
    
    [SerializeField] int runDistance;
    [SerializeField] float stunDuration;
    bool hasPlayedStepSound = false;
    
    bool isRunningAway = false;
    bool isStunned;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource; 
    [SerializeField] AudioClip screamClip;
    [Range(0f, 1f)] public float demonVolume = 0.5f;
    [SerializeField] private AudioSource warningSource;
    [SerializeField] private AudioClip warningClip;
    [SerializeField] private float maxHearDistance = 15f;
    [Range(0f, 1f)][SerializeField] private float warningVolume = 0.5f;

    [SerializeField] private float fadeSpeed = 2f;
    private bool isPlayingWarning = false;
    [SerializeField] private float chaseCooldown = 10f; 
    private bool isCoolingDown = false;
    private float cooldownTimer = 0f;
    [SerializeField] private float detectionRange = 20f; // how far the demon can see the player
    [SerializeField] private float fieldOfView = 120f;  // angle of vision




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
        if (!isRunningAway && CanSeePlayer())
        {
            agent.isStopped = false;
            isStunned = false;
            isRunningAway = false;
            hasChosenRunDest = false;
            chasePlayer();
            return;
        }
        if (isRunningAway)
        {
            HandleRunningAway();
        }
        else if (isCoolingDown)
        {
            cooldownTimer += Time.deltaTime;
            roam(); // demon wanders during cooldown

            if (cooldownTimer >= chaseCooldown)
            {
                isCoolingDown = false; // done roaming
            }
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
        UpdateAnimation();

    }
    protected override void roam()
    {
        base.roam();
        if (CanSeePlayer())
        {
            chasePlayer();
        }
    }
    protected override void chasePlayer()
    {

        base.chasePlayer();
        if (isRunningAway || isCoolingDown || !CanSeePlayer())
        {
            if (isPlayingWarning)
            {
                FadeOutWarning();
            }
            return;
        }

        if (warningSource == null || warningClip == null) return;

        float targetVolume = 0f;

        if (agent.remainingDistance <= maxHearDistance)
        {
            targetVolume = Mathf.Clamp01(1 - (agent.remainingDistance / maxHearDistance));
            if (!isPlayingWarning)
            {
                warningSource.pitch = 1.3f;
                warningSource.volume = warningVolume;
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
    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = gameManager.instance.player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > detectionRange) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView / 2f) return false;

        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, directionToPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
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
            agent.speed = Mathf.Lerp(agent.speed, 0f, Time.deltaTime * 2f);

            if (stunTimer >= stunDuration)
            {
                isStunned = false;
                isRunningAway = false; 
                agent.isStopped = false;
                hasChosenRunDest = false;
                StartCooldown();
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
            StartCooldown();
        }
    }
    void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = 0f;
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
            anim.speed = Mathf.Lerp(anim.speed, 0, Time.deltaTime * 3);

        }
    }
    void PlayScream()
    {
        if (audioSource != null && screamClip != null)
        {
            audioSource.PlayOneShot(screamClip,demonVolume);
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

    bool isPlayerOnNavMesh()
    {
        NavMeshHit hit;

        return NavMesh.SamplePosition(gameManager.instance.player.transform.position, out hit, 1.0f, NavMesh.AllAreas);
    }

    void FadeOutWarning()
    {
        warningSource.volume = Mathf.Lerp(warningSource.volume, 0f, Time.deltaTime * fadeSpeed);
        if (warningSource.volume <= 0.01f)
        {
            warningSource.Stop();
            isPlayingWarning = false;
        }
    }


}