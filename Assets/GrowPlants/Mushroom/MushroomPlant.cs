using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MushroomPlant : MonoBehaviour, IGrowablePlant
{
    private Animator animator;
    public bool mushdb = false;
    public GameObject Collide;
    public bool candie = false;
    private BoxCollider2D platform;
    private int spirits = 1;
    public int spiritCost => spirits;
    [SerializeField] private bool isFake = false;
    [SerializeField] private GameObject mushMimic;
    public void setWaterGrow(bool value)
    {
        waterGrown = value;
    }
    public bool waterGrown = false;
    public bool WaterGrown => waterGrown;

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
                if (!isFake)
                {
                    StartCoroutine(GrowCycle());
                } else
                {
                    StartCoroutine(MimicGrow());
                }
                
            }
        }
    }

    public void Die()
    {
        if (mushdb == true)
        {
            if (candie == true)
            {
                if (!isFake)
                {
                    StartCoroutine(diecycle());
                } 
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

    private IEnumerator MimicGrow()
    {
        ParticleSystem debris = gameObject.GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule debrisEmission = debris.emission;
        debrisEmission.enabled = true;
        candie = false;
        animator.SetTrigger("Grow");
        yield return new WaitForSeconds(0.45f);
        Instantiate(mushMimic);
        Destroy(gameObject);
    }
}