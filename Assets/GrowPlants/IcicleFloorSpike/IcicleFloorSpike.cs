using System.Collections;
using UnityEngine;

public class IcicleFloorSpike : MonoBehaviour, IGrowablePlant
{
    /* ICICLEFLOORSPIKE
     * Handles growing of iciclefloorspike
     * Handles damaging enemies when walked over
     * Handles death of iciclefloorspike
     */

    private Animator animator;
    public static bool candamage = false;

    //---- INTERFACE ----
    public bool icicledb = false;
    public bool candie = false;
    private int spirits = 1;
    public int spiritCost => spirits;
    
    public bool IsGrown => icicledb;
    public bool CanDie => candie;

    //START - Gets animator component

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /* FUNCTIONS 
     * Grow is called through druridgrowframework when clicking on iciclefloorspike
     * Die is called through druidgrowframework when click on a grown iciclefloorspike
     * OnTriggerEnter2D manages enemies stepping on floorspike through the persistence script's damagemanager
     */
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered by: " + collision.name);
        if (collision && candamage)
        {
            IDamageAble enemy = collision.gameObject.GetComponent<IDamageAble>();
            if (enemy != null)
            {
                Debug.Log(collision.gameObject.name + " has stepped over IcicleSpike!");
                Persistence.instance.ApplyDamage(collision.gameObject, 2f);
            }
        }
    }

    /* Coroutines
     * Growcycle manages grow animations for the icicle floor spike
     * Diecycle manages the death animations for the icicle floor spike
     */
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

}