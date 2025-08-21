using System.Collections;
using UnityEngine;

public class GlowRootPlant : MonoBehaviour
{
    private Animator animator;
    public bool glowdb = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GrowGlowRoot()
    {
        if (glowdb == false)
        {
            StartCoroutine(GrowCycle());
        }
    }

    private IEnumerator GrowCycle()
    {
        animator.SetTrigger("Grow");
        glowdb = true;

        yield return new WaitForSeconds(10f);
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(3f);
        glowdb = false;
    }
}