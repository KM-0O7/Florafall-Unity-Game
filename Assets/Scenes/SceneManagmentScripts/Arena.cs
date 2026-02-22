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
    [SerializeField] private BoxCollider2D[] walls;
     
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
            for (int j = 0; j < numberOfEnemies; j ++)
            {
                spawnEnemy();
            }
            yield return new WaitUntil(() => aliveEnemies.Count == 0);
            yield return new WaitForSeconds(timeBetweenWaves);   
        }
        onComplete();
    }

    private void spawnEnemy()
    {
        var clonedEnemy = Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)]);
        IEnemy enemyComponent = clonedEnemy.GetComponent<IEnemy>();
        if (enemyComponent != null)
        {
            aliveEnemies.Add(enemyComponent);

            if (enemyComponent.FlyingEnemy) clonedEnemy.transform.position = airSpawnLocations[Random.Range(0, airSpawnLocations.Length)].transform.position;
            else if (enemyComponent.GroundEnemy) clonedEnemy.transform.position = groundSpawnLocations[Random.Range(0, groundSpawnLocations.Length)].transform.position;
        }
    }

    private void onComplete()
    {
        inArena = false;
        arenaCompleted = true;
        aliveEnemies.Clear();
        for (int i = 0; i < walls.Length; i++)
        {
            BoxCollider2D wallCollider = walls[i].GetComponent<BoxCollider2D>();
            Animator wallAnimator = walls[i].GetComponent<Animator>();
            if (wallCollider != null) wallCollider.enabled = false;
            if (wallAnimator != null) wallAnimator.SetTrigger("Open");
        }
    }

    private IEnumerator spawnRoutine()
    {
        inArena = true;
        FollowPlayer followPlayer = Camera.main.GetComponent<FollowPlayer>();
        followPlayer.SetBounds(arenaBounds.bounds.min, arenaBounds.bounds.max);
        for (int i = 0; i < walls.Length; i++)
        {
            BoxCollider2D wallCollider = walls[i].GetComponent<BoxCollider2D>();
            Animator wallAnimator = walls[i].GetComponent<Animator>();
            if (wallCollider != null) wallCollider.enabled = true;
            if (wallAnimator != null) wallAnimator.SetTrigger("Close");
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(mainSpawnFrameWork());
    }
}
