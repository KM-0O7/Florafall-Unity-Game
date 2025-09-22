using System.Collections;
using UnityEngine;

public class ExplosionShroom : MonoBehaviour, IGrowablePlant
{
    public bool explosiondb = false;
    public bool candie = false;
    Animator animator;
    public bool IsGrown => explosiondb;
    public bool CanDie => candie;


    public void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Grow()
    {
        if (explosiondb == false)
        {
            if (candie == false)
            {
                StartCoroutine(GrowCycle());
            }
        }
    }

    IEnumerator GrowCycle()
    {
        animator.SetTrigger("grow");
        yield return new WaitForSeconds(1f);
    }

    public void Die()
    {

    }
}
