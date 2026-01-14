using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private bool druidDamaging = false;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private float damage = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && druidDamaging)
        {
            Persistence.instance.ApplyDamageToDruid(collision.gameObject, damage);
        } else if ((collision.gameObject.layer == LayerMask.NameToLayer("GrowEnemy") || collision.gameObject.layer == LayerMask.NameToLayer("RoboticEnemy")) && !druidDamaging)
        {
            if (!parentObject)
            {
                Persistence.instance.ApplyDamage(collision.gameObject, damage);
            }  
        }
    }
}
