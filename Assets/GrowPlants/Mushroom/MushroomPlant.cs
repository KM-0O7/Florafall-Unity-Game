using System.Collections;
using UnityEngine;

public class MushroomPlant : MonoBehaviour
{
    private Animator animator;
    public bool mushdb = false;
    public GameObject Collide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void GrowMush()
    {
        if (mushdb == false)
        {
            StartCoroutine(GrowCycle());
        }
    }

    private IEnumerator GrowCycle()
    {
        animator.SetTrigger("Grow");
        mushdb = true;
        yield return new WaitForSeconds(0.75f);
        Collide.AddComponent<BoxCollider2D>();
        BoxCollider2D platform = Collide.GetComponent<BoxCollider2D>();
        platform.enabled = true;
        platform.usedByEffector = true;

        yield return new WaitForSeconds(10f);
        animator.SetTrigger("Die");
        Destroy(platform);

        yield return new WaitForSeconds(3f);
        mushdb = false;
    }
}