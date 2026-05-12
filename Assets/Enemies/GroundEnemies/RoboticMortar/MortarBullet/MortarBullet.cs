using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MortarBullet : MonoBehaviour, IGrowablePlant
{
    private bool growDB = false;
    private bool canDie = false;
    private bool isGrown = false;
    private bool canGrow = true;
    private bool alreadyGrown = false;
    public bool IsGrown => growDB;
    public bool CanDie => canDie;

    public int spiritCost => 1;
    public bool waterGrown = false;
    public bool WaterGrown => waterGrown;
    public void setWaterGrow(bool value)
    {
        waterGrown = value;
    }

    [SerializeField] private int explodeDamageToEnemies = 2;
    [SerializeField] private float explodeTime = 2.5f;
    [SerializeField] private GameObject explosionHitbox;

    void Start()
    {
        
    }

    
    void Update()
    {
        if (isGrown)
        {

        }
    }
    public void Grow()
    {
        if (!growDB && !canDie && canGrow)
        {
            growDB = true;
            canDie = true;
            isGrown = true;
            if (!alreadyGrown)
            {
                alreadyGrown = true;
            }
        }
    }

    public void Die()
    {
        if (growDB && canDie && canGrow)
        {
            growDB = false;
            canDie = false;
            isGrown = false;
        }
    }

    //---- BOTTOM EXPLOSION ----
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!growDB && !alreadyGrown)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(Explode());
            }  
        }
    }

    private IEnumerator Explode()
    {
        explosionHitbox.SetActive(true);
        yield return null;
    }

}
