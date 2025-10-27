using System.Collections;
using UnityEngine;

public class GlowRootPlant : MonoBehaviour, IGrowablePlant
{
    private Animator animator;
    public bool glowdb = false;
    public bool candie = false;
    private int spirits = 1;
    public int spiritCost => spirits;
    public bool IsGrown => glowdb;
    public bool CanDie => candie;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Grow()
    {
        if (glowdb == false)
        {
            StartCoroutine(GrowCycle());
        }
    }

    public void Die()
    {
        if (glowdb == true)
        {
            if (candie == true)
            {
                StartCoroutine(diecycle());
            }
        }
    }

    private IEnumerator GrowCycle()
    {
        animator.SetTrigger("Grow");
        glowdb = true;
        yield return new WaitForSeconds(0.75f);
        candie = true;
    }

    private IEnumerator diecycle()
    {
        candie = false;
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(3f);
        animator.SetTrigger("dbdone");
        glowdb = false;
    }
}