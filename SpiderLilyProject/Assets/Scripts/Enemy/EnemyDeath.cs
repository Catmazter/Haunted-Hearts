using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyDeath : EnemyBase
{

    // ====timer==== //
    [SerializeField] float selfDestruct = 5f; //time player has to be inside safezone before enemy destroys
    [SerializeField] float escapeDistance = 10f;

    float timerChase = 3.0f;
    Coroutine killEnemy;

    DetectSafeZone detector; //player in safezone trigger

    // ====audio==== //

    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip[] audChase;
    [UnityEngine.Range(0, 1)][SerializeField] float audChaseVol;

    [SerializeField] AudioClip[] audDestroy;
    [UnityEngine.Range(0, 1)][SerializeField] float audDestroyVol;



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
        base.Update();

        if (PlayerSafe())
        {
            EscapefromPlayer();

            //start destroy timer
            if (killEnemy == null)
            {
                killEnemy = StartCoroutine(IfEnemySafe());
            }

        }
        else
        {
            //don't destroy if enemy out
            if (killEnemy != null)
            {
                StopCoroutine(killEnemy);
                killEnemy = null;
            }

            chasePlayer();
        }

        if (timerChase > 0)
        {
            timerChase -= Time.deltaTime;
            return;
        }
    }

    IEnumerator IfEnemySafe()
    {

        yield return new WaitForSeconds(selfDestruct);

        if (PlayerSafe())
        {

            if (audDestroy.Length > 0)
                aud.PlayOneShot(audDestroy[Random.Range(0, audDestroy.Length)], audDestroyVol);

            Destroy(gameObject);
        }
            killEnemy = null;
    }

    private bool PlayerSafe()
    {
        return (detector != null && detector.InSafeZone);
    }

    void EscapefromPlayer()
    {
        if (agent == null || gameManager.instance.player == null) return;

        Vector3 dirAway = (transform.position - gameManager.instance.player.transform.position).normalized;
        Vector3 escapePos = transform.position + dirAway * escapeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(escapePos, out hit, escapeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            agent.speed = chaseSpeed * 0.8f; //slower than chase
        }
    }

    protected override void chasePlayer()
    {
        agent.speed = chaseSpeed;
        base.chasePlayer();

        aud.PlayOneShot(audChase[UnityEngine.Random.Range(0, audChase.Length)], audChaseVol);

    }

    public void ChaseNow()
    {
        timerChase = 0;
    }

   /* bool PlayerOnNavMesh()
    {
        return NavMesh.SamplePosition(gameManager.instance.player.transform.position, out _, playerNavmeshRadius, NavMesh.AllAreas);
        //checks around player if theres navmesh
        didn't worked: too many spots without navmesh (not reliable)
    }
   */
}
