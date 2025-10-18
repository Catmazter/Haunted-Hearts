using UnityEngine;

public class SpawnerDeath : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform spawnPos;

    //timer
    [SerializeField] float spawnTime = 5.0f;
    [SerializeField] GameObject[] wallSafeZone;

    float timer;
    bool counting; //waiting until re-spawn 
    private int gamecount = 0;

    EnemyDeath status;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startCounting(); //first timer going
    }

    // Update is called once per frame
    void Update()
    {
        //no enemy present
        if (status == null)
        {
            if (!counting) //if enemy is destroyed
            {
                startCounting(); //restart timer
            }

            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                return;
            }

                spawnEnemy();
                counting = false;

            if (gamecount == 3)
            {
                for (int i = 0; i < wallSafeZone.Length; i++)
                {
                    wallSafeZone[i].SetActive(true);
                }
            }
        }
    }

    void spawnEnemy()
    {
        Vector3 pos;
        Quaternion rot;

        if (spawnPos != null)
        {
            pos = spawnPos.position;
            rot = spawnPos.rotation;
        }
        else
        {
            pos = transform.position;
            rot = transform.rotation;
        }

        GameObject alive = Instantiate(objectToSpawn, pos, rot);

        status = alive.GetComponent<EnemyDeath>();

        gamecount += 1;

        if ( status != null )
        {
            status.ChaseNow();
        }

    }

   void startCounting()
    {
        timer = spawnTime;
        counting = true; //timer has started!
    }

}
