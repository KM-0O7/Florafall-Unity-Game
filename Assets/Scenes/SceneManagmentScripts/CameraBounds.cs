using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    Arena arena;

    private void Start()
    {
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (arena.inArena == false)
            {
                // get bounds of this room’s collider
                BoxCollider2D bc = GetComponent<BoxCollider2D>();
                Bounds b = bc.bounds;

                Vector2 min = b.min;
                Vector2 max = b.max;

                Camera.main.GetComponent<FollowPlayer>().SetBounds(min, max);
            }  
        }
    }
}