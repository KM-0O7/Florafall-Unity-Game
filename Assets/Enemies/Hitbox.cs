using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Persistence.instance.ApplyDamageToDruid(collision.gameObject, 1f);
        }
    }
}
