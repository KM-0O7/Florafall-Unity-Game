using System.Collections;
using UnityEngine;

public class IcicleFloorSpike : MonoBehaviour, IGrowablePlant
{
    private Animator animator;
    public bool icicledb = false;
    public bool candie = false;
    public static bool candamage = false;
    private bool damagecd = false;
    private int spirits = 1;
    public int spiritCost => spirits;

    //interface
    public bool IsGrown => icicledb;

    public bool CanDie => candie;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Grow()
    {
        if (!icicledb)
        {
            if (!candie)
            {
                StartCoroutine(GrowCycle());
            }
        }
    }

    public void Die()
    {
        if (icicledb)
        {
            if (candie)
            {
                StartCoroutine(DieCycle());
            }
        }
    }

    private IEnumerator GrowCycle()
    {
        animator.SetTrigger("Grow");
        icicledb = true;

        yield return new WaitForSeconds(0.75f);
        candamage = true;
        candie = true;
    }

    private IEnumerator DieCycle()
    {
        candie = false;
        candamage = false;
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(3f);
        icicledb = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && candamage && !damagecd)
        {
            if (collision.gameObject.CompareTag("GrowableEnemy"))
            {
                IGrowableEnemy enemy = collision.gameObject.GetComponent<IGrowableEnemy>();
                if (enemy != null)
                {
                    if (!enemy.Dead)
                    {
                        damagecd = true;
                        enemy.TakeDamage(2f);
                        StartCoroutine(DamageCooldown());
                        Debug.Log(collision.gameObject.name + "walked on" + gameObject.name);
                    }
                }
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(1f);
        damagecd = false;
    }
}