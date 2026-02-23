using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    Arena arena;
    BoxCollider2D bc;
    private void Start()
    {
         bc = GetComponent<BoxCollider2D>();
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (arena.inArena == false)
            {
                
                ResetBounds();
            }  
        }
    }

    public void ResetBounds()
    {
      
        Bounds b = bc.bounds;

        Vector2 min = b.min;
        Vector2 max = b.max;

        Camera.main.GetComponent<FollowPlayer>().SetBounds(min, max);
    }
}