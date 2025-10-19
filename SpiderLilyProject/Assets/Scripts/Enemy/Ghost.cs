using NUnit.Framework.Internal.Builders;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    [SerializeField] NavMeshAgent ghost;
    [SerializeField] SkinnedMeshRenderer render;
    [SerializeField] Transform headPos;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTimer;
    [SerializeField] int ghostDamage;
    [SerializeField] float ghostAttackCD;
    [SerializeField] float ghostAttackRange;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] ghostNoise;
    [Range(0, 1)][SerializeField] float ghostNoiseVol;

    Vector3 startPos;
    float ghostNoiseTimer;
    float ghostSpeedOrig;
    float ghostAttackCDOrig;
    float hearingLevel;
    float increasedSpeedNoiseLevel;
    float roamTimer;
    float stoppingDistOrig;
    bool isPlayerInTrigger;
  //  bool isVisible;
   // bool isPlayingNoise;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<SkinnedMeshRenderer>();
        render.enabled = false;
        ghost.speed = gameManager.instance.playerScript.speed;
        ghostSpeedOrig = ghost.speed;
        ghostAttackCDOrig = ghostAttackCD;
        startPos = transform.position;
        stoppingDistOrig = ghost.stoppingDistance;
        hearingLevel = 5e-7f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ghost.isStopped)
        {
            roamTimer += Time.deltaTime / 3;
        }
        if (isPlayerInTrigger && AudioPeer.currentAMP < hearingLevel)
        {
            checkRoam();
        }
        else if (!isPlayerInTrigger && AudioPeer.currentAMP < hearingLevel)
        {
            checkRoam();
        }
        else
        {
            attackPlayer();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        { 
            isPlayerInTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }
    void attackPlayer()
    {
        attackCD();
        ghostNoiseCD();
        Vector3 targetPos = gameManager.instance.player.transform.position - headPos.position;
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, targetPos, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                ghost.SetDestination(gameManager.instance.player.transform.position);
                if (gameManager.instance.playerScript.isSprinting)
                {
                    ghost.speed *= gameManager.instance.playerScript.speed / ghost.speed;
                }
                else if (!gameManager.instance.playerScript.isSprinting && ghost.speed > gameManager.instance.playerScript.speed)
                {
                    ghost.speed = ghostSpeedOrig;
                }
                if (isPlayerInTrigger)
                {
                    StartCoroutine(ghostVisible());
                    if (ghostNoiseTimer <= 0)
                    {
                        StartCoroutine(ghostSounds());
                        ghostNoiseTimer = 2.25f;
                    }
                    if (AudioPeer.currentAMP >= hearingLevel)
                    {
                        if (ghost.remainingDistance <= ghost.stoppingDistance)
                        {
                            faceTarget(targetPos);
                            if (ghostAttackCD <= 0.0f && ghostAttackRange >= ghost.remainingDistance)
                            {
                                
                                gameManager.instance.playerScript.takeDamage(ghostDamage);
                                ghostAttackCD = ghostAttackCDOrig;
                            }
                        }
                        ghost.stoppingDistance = stoppingDistOrig;
                    }
                    else
                    {
                        ghost.isStopped = true;
                    }
                    ghost.isStopped = false;
                }
            }
        }
    }
    void ghostNoiseCD()
    {
        if (ghostNoiseTimer > 0)
        {
            ghostNoiseTimer -= Time.deltaTime;
        }
    }
    void faceTarget(Vector3 dir)
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, transform.position.y, dir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    void attackCD()
    {
        if (ghostAttackCD > 0.0f)
        {
            ghostAttackCD -= Time.deltaTime;
        }
    }
    void checkRoam()
    {
        if (roamTimer >= roamPauseTimer && !ghost.isStopped)
        {
            roam();
            if (isPlayerInTrigger)
            {
                StartCoroutine(ghostVisible());
                StartCoroutine(ghostSounds());
            }
        }
    }
    void roam()
    {
        roamTimer = 0;
        ghost.stoppingDistance = 0;
        Vector3 randomPos = Random.insideUnitSphere * roamDist;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        ghost.SetDestination(hit.position);
    }
    IEnumerator ghostVisible()
    {
       // isVisible = true;
        render.enabled = true;
        if (isPlayerInTrigger)
        {
            yield return new WaitForSeconds(0.3f);
        }
        render.enabled = false;
     //   isVisible = false;
    }
    IEnumerator ghostSounds()
    {
       // isPlayingNoise = true;
        aud.PlayOneShot(ghostNoise[Random.Range(0, ghostNoise.Length)], ghostNoiseVol);
        if (isPlayerInTrigger)
        {
            yield return new WaitForSeconds(2.25f);
        }
       // isPlayingNoise = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Moveable"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        }
    }
}
