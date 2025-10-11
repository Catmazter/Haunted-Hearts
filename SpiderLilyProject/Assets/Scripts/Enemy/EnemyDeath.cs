using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyDeath : EnemyBase
{
    [SerializeField] Animator anim;

    // ====timer==== //
    [SerializeField] float selfDestruct = 5f; //time player has to be inside safezone before enemy destroys

    float timerChase = 3.0f;

    Coroutine killEnemy;

    DetectSafeZone detector; //player in safezone trigger

    // ====audio==== //

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audChase;
    [UnityEngine.Range(0, 1)][SerializeField] float audChaseVol;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        detector = gameManager.instance.player.GetComponent<DetectSafeZone>();
        // detector = gameManager.instance.PlayerInSafeZone
    }

    // Update is called once per frame
    protected override void Update()
    {

        if (detector != null && detector.InSafeZone)
        {
            if (killEnemy == null)
            {
                killEnemy = StartCoroutine(IfEnemySafe());
            }
        }
        else
        {
            if (killEnemy != null)
            {
                StopCoroutine(killEnemy);
                killEnemy = null;
            }
        }

        //if (detector)
        //{
        //    if (killEnemy == null) killEnemy = StartCoroutine(IfEnemySafe());
        //}
        //else
        //{
        //    if (killEnemy != null) { StopCoroutine(killEnemy); killEnemy = null; }
        //}


        startChasing();
    }

    IEnumerator IfEnemySafe()
    {
        yield return new WaitForSeconds(selfDestruct);

        if (detector != null && detector.InSafeZone)
        {
            Destroy(gameObject);
            killEnemy = null;
        }
    }

    protected override void chasePlayer()
    {
        agent.speed = chaseSpeed;
        base.chasePlayer();

        aud.PlayOneShot(audChase[UnityEngine.Random.Range(0, audChase.Length)], audChaseVol); 

    }

    private void startChasing()
    {

        if (timerChase > 0)
        {
            timerChase -= Time.deltaTime;
            return;
        }

        chasePlayer();
    }

    public void ChaseNow()
    {
        timerChase = 0;
    }

    //bool PlayerOnNavMesh()
    //{
    //    return NavMesh.SamplePosition(gameManager.instance.player.transform.position, out _, playerNavmeshRadius, NavMesh.AllAreas); 
    //    //checks around player if theres navmesh
    //didn't worked: too many spots without navmesh (not reliable)
    //}
}
