using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BirdSpawner : MonoBehaviour
{

    [SerializeField] private GameObject bird;
    [SerializeField] private float birdFlightSpeed = 2f;
    [SerializeField] private float positionOffSet = 2f;
    [SerializeField] private float timeBetweenSpawns = 15f;
    [SerializeField] private bool movingLeft = true;
    [SerializeField] private Transform moveToPos;
    private GameObject birdClone;
    private bool canSpawn = true;
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (canSpawn)
            {
                canSpawn = false;
                birdClone = Instantiate(bird, gameObject.transform);
                birdClone.SetActive(true);
                birdClone.transform.position = gameObject.transform.position + new Vector3(0, Random.Range(positionOffSet, positionOffSet), 0);
                Debug.Log("Moved");

                while (Vector2.Distance(birdClone.transform.position, moveToPos.position) > 0.5)
                {
                    birdClone.transform.position = Vector2.MoveTowards(birdClone.transform.position, new Vector2(moveToPos.position.x, birdClone.transform.position.y), birdFlightSpeed * Time.deltaTime);
                    yield return null;
                    
                }
                Destroy(birdClone.gameObject);
                yield return new WaitForSeconds(timeBetweenSpawns);
                canSpawn = true;
            } 
        }
    }
}
