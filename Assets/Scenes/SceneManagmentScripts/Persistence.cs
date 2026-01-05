using UnityEngine;

public class Persistence : MonoBehaviour
{
    /* PERSISTENCE
     * This script handles most persistent data except for chunk loader
     * Handles damage via calling Persistence.instance.ApplyDamage(gameobject, damage);
     * Call this to damage any class with the interface IDamageAble
     */

    public static Persistence instance;

    private GameObject druid;

    //---- PERSISTENCE FRAMEWORK ----
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        druid = GameObject.FindGameObjectWithTag("Player");
    }

    //---- DAMAGE MANAGER ----
    public void ApplyDamage(GameObject target, float amount)
    {
        IDamageAble damageable = target.GetComponent<IDamageAble>();
        if (damageable != null && !damageable.Dead && target != druid)
        {
            damageable.TakeDamage(amount);
            Debug.Log("PersistentGameManager: " + target.name + " took " + amount + "  damage.");
        }
    }
}