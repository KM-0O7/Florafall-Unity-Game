using UnityEngine;
using System.Collections;

public class MossyBush : MonoBehaviour, IEnemy, IGrowableEnemy
{
    public bool CantGrow => cantGrow;
    private bool cantGrow = false;
    private EnemyDamage damage;
    public bool Dead => damage.dead;

    private bool growDb;
    public bool IsGrown => growDb;
    private bool candie = false;
    public bool FlyingEnemy => true;
    private bool isLerping = false;
    public bool IsLerping => isLerping;
    public void SetLerp(bool value)
    {
        isLerping = value;
    }
    public bool GroundEnemy => false;
    public int spiritCost => 4;
    public bool CanDie => candie;

    void Start()
    {
        damage = GetComponent<EnemyDamage>();
    }


    void Update()
    {

    }

    public void Grow()
    {

    }

    public void Die()
    {

    }
}
