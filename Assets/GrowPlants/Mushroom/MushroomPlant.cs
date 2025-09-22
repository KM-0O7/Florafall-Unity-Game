using System.Collections;
using UnityEngine;

public class MushroomPlant : MonoBehaviour, IGrowablePlant
{
    private Animator animator;
    public bool mushdb = false;
    public GameObject Collide;
    public bool candie = false;
    private BoxCollider2D platform;

    public bool IsGrown => mushdb;
    public bool CanDie => candie;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Grow()
    {
        if (mushdb == false)
        {
            if (candie == false)
            {
                StartCoroutine(GrowCycle());
            }
        }
    }

    public void Die()
    {
        if (mushdb == true)
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
        mushdb = true;
        yield return new WaitForSeconds(0.75f);
        candie = true;
        Collide.AddComponent<BoxCollider2D>();
        platform = Collide.GetComponent<BoxCollider2D>();
        platform.enabled = true;
        platform.usedByEffector = true;
    }

    private IEnumerator diecycle()
    {
        candie = false;
        animator.SetTrigger("Die");
        Destroy(platform);

        yield return new WaitForSeconds(3f);
        animator.SetTrigger("dbdone");
        mushdb = false;
    }
}