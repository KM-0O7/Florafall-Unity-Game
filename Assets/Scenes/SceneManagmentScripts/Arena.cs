using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] private int numberOfWaves = 3;
    [SerializeField] private int numberOfEnemies = 2;
    [SerializeField] private float timeBetweenWaves = 2f;
    [SerializeField] private GameObject[] enemiesToSpawn;
    [SerializeField] private Transform[] groundSpawnLocations;
    [SerializeField] private Transform[] airSpawnLocations;
    [SerializeField] private GameObject[] walls;
    [SerializeField] private float yAirSpawnDistance = 10;
    [SerializeField] private float lerpTime = 2f;
     
    private HashSet<IEnemy> aliveEnemies = new HashSet<IEnemy>();
    private BoxCollider2D arenaBounds;
    public bool arenaCompleted = false;
    public bool inArena = false;

    void Start()
    {
        arenaBounds = GetComponent<BoxCollider2D>();
    }

 
    void Update()
    {
        aliveEnemies.RemoveWhere(enemy => enemy.Dead);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (arenaCompleted || inArena) return;

        if (collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(spawnRoutine());
            }
        }
    }

    private IEnumerator mainSpawnFrameWork() 
    {
        for (int i = 0; i < numberOfWaves; i++)
        {
            List<Transform> activeGroundSpots = new List<Transform>(groundSpawnLocations);
            List<Transform> activeAirSpots = new List<Transform>(airSpawnLocations);
            for (int j = 0; j < numberOfEnemies; j ++)
            {
                StartCoroutine(spawnEnemy(activeGroundSpots, activeAirSpots));
            }
            yield return new WaitUntil(() => aliveEnemies.Count == 0);
            yield return new WaitForSeconds(timeBetweenWaves);   
        }
        onComplete();
    }

    private IEnumerator spawnEnemy(List<Transform> groundSpots, List<Transform> airSpots)
    {
        var clonedEnemy = Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)]);
        clonedEnemy.SetActive(true);
        
        IEnemy enemyComponent = clonedEnemy.GetComponent<IEnemy>();
        if (enemyComponent != null)
        {
            aliveEnemies.Add(enemyComponent);

            if (enemyComponent.FlyingEnemy)
            {
                if (airSpots.Count > 0)
                {
                    Transform spawnPointPosition = airSpots[Random.Range(0, airSpots.Count)].transform;
                    BoxCollider2D clonedBox = clonedEnemy.GetComponent<BoxCollider2D>();
                    clonedBox.enabled = false;
                    clonedEnemy.transform.position = spawnPointPosition.position + new Vector3(Random.Range(-3, 3), yAirSpawnDistance, 0);
                    airSpots.Remove(spawnPointPosition);
                    enemyComponent.SetLerp(true);
                    float t = 0;
                    Vector2 startPosition = clonedEnemy.transform.position;
                    while (t < lerpTime)
                    {
                        yield return null;
                        float easing = Mathf.SmoothStep(0f, 1f, t);
                        t += Time.deltaTime;
                        clonedEnemy.transform.position = Vector2.Lerp(startPosition, spawnPointPosition.transform.position, easing);

                    }
                    enemyComponent.SetLerp(false);
                    clonedBox.enabled = true;
                }
            }

            else if (enemyComponent.GroundEnemy)
            {
                Transform spawnPointPosition = groundSpots[Random.Range(0, groundSpots.Count)];
                clonedEnemy.transform.position = spawnPointPosition.position;
                groundSpots.Remove(spawnPointPosition);
            }
        }
    }

    private void onComplete()
    {
        inArena = false;
        arenaCompleted = true;
        aliveEnemies.Clear();
       /* for (int i = 0; i < walls.Length; i++)
        {
            BoxCollider2D wallCollider = walls[i].GetComponent<BoxCollider2D>();
            Animator wallAnimator = walls[i].GetComponent<Animator>();
            if (wallCollider != null) wallCollider.enabled = false;
            if (wallAnimator != null) wallAnimator.SetTrigger("Open");
        }*/
    }

    private IEnumerator spawnRoutine()
    {
        inArena = true;
        FollowPlayer followPlayer = Camera.main.GetComponent<FollowPlayer>();
        followPlayer.SetBounds(arenaBounds.bounds.min, arenaBounds.bounds.max);
       /* for (int i = 0; i < walls.Length; i++)
        {
            BoxCollider2D wallCollider = walls[i].GetComponent<BoxCollider2D>();
            Animator wallAnimator = walls[i].GetComponent<Animator>();
            if (wallCollider != null) wallCollider.enabled = true;
            if (wallAnimator != null) wallAnimator.SetTrigger("Close");
        }*/
        yield return new WaitForSeconds(1);
        StartCoroutine(mainSpawnFrameWork());
    }
}
