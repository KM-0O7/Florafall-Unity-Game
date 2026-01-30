using UnityEngine;

public class IceCrowBulletDetection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(" Ice Bullet Hit Player!");
            Persistence.instance.ApplyDamageToDruid(collision.gameObject, 1f);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Breakable"))
        {
            Debug.Log("Ice Bullet Hit Ground!");
            Destroy(gameObject);
        }
    }
}
