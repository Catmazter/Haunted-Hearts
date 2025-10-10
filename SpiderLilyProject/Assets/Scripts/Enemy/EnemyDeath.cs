using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyDeath : EnemyBase
{
    [SerializeField] Animator anim;

   float timerChase = 3.0f;

    [SerializeField] float selfDestruct = 5f; //time player has to be inside safezone before enemy destroys
    Coroutine killEnemy;

    DetectSafeZone detector; //player in safezone trigger


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        detector = gameManager.instance.player.GetComponent<DetectSafeZone>();
        // dtector = gameManager.instance.PlayerInSafeZone

        if (!detector)
            Debug.LogWarning("EnemyDeath: No encontré SafeZoneDetector en el Player.");
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
