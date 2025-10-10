using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeath : EnemyBase
{
    [SerializeField] Animator anim;

    [SerializeField] float playerNavmeshRadius = 1.0f;
    [SerializeField] float selfDestruct = 5f;

    float timerChase = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        

        startChasing();

    }

    protected override void chasePlayer()
    {
        agent.speed = chaseSpeed;
        base.chasePlayer();

    }

    //bool PlayerOnNavMesh()
    //{
    //    return NavMesh.SamplePosition(gameManager.instance.player.transform.position, out _, playerNavmeshRadius, NavMesh.AllAreas); 
    //    //checks around player if theres navmesh
            //didn't worked: too many spots without navmesh (not reliable)
    //}

    private void startChasing()
    {

        if (timerChase > 0)
        {
            timerChase -= Time.deltaTime;
            return;
        }

        chasePlayer();
    }
}
