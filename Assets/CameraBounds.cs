using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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