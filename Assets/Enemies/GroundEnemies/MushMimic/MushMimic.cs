using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class MushMimic : MonoBehaviour, IGrowableEnemy, IEnemy
{
    private bool isGrown = false;
    private bool canDie = false;
    private bool dead = false;
    [SerializeField] private int cost;
    EnemyDamage damage;
    public bool IsGrown => isGrown;
    public int spiritCost => cost;
    public bool Dead => damage.dead;
    public bool FlyingEnemy => false;
    private bool isLerping = false;
    public bool IsLerping => isLerping;
    public void SetLerp(bool value)
    {
        isLerping = value;
    }
    public bool GroundEnemy => true;

    public bool CanDie => canDie;

    public float flashDuration = 0.3f;

    public float flashPeak = 1f;
    [SerializeField] private float movementSpeed = 2f;
    GameObject druid;
    private SpriteRenderer spriteRenderer;

 
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        druid = GameObject.FindGameObjectWithTag("Player");
        damage = GetComponent<EnemyDamage>();
    }
    void Update()
    {
        if (druid.transform.position.x > gameObject.transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else spriteRenderer.flipX = false;
    }

    public void Grow()
    {
        if (!damage.dead)
        {
            if (IsGrown == false)
            {
                if (canDie == false)
                {
                    isGrown = true;
                    canDie = true;
                    StartCoroutine(GrowCycle());
                }
            }
        }
    }

    public void Die()
    {
        if (!damage.dead)
        {
            if (isGrown == true)
            {
                if (canDie == true)
                {
                    isGrown = false;
                    canDie = false;
                    StartCoroutine(DieCycle());
                }
            }
        }
    }

    private IEnumerator GrowCycle()
    {
        yield return null;
    }

    private IEnumerator DieCycle()
    {
        yield return null;
    }

}
