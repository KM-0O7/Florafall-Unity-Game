using System.Collections;
using UnityEngine;
using Pathfinding;

public class IceWall : MonoBehaviour, IGrowablePlant
{
    [SerializeField] private int growthCost = 2;
    private bool isGrown = false;
    private bool canDie = false;
    public bool CanDie => canDie;
    public bool IsGrown => isGrown;
    public int spiritCost => growthCost;
    private Animator animator;
    [SerializeField] private BoxCollider2D wallCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Grow()
    {
        if (!canDie && !isGrown) StartCoroutine(GrowCycle());
    }

    public void Die()
    {
        if (canDie && isGrown) StartCoroutine(DieCycle());
    }

    private IEnumerator GrowCycle()
    {
        isGrown = true;
        animator.SetTrigger("Grow");
        yield return new WaitForSeconds(0.4f);
        wallCollider.enabled = true;
        canDie = true;

        if (wallCollider != null)
        {
            Bounds bounds = wallCollider.bounds;
            GraphUpdateObject guo = new GraphUpdateObject(bounds);
            guo.updatePhysics = true;
            AstarPath.active.UpdateGraphs(guo);
        }
    }

    private IEnumerator DieCycle()
    {
        isGrown = false;
        wallCollider.enabled = false;
        if (wallCollider != null)
        {
            Bounds bounds = wallCollider.bounds;
            GraphUpdateObject guo = new GraphUpdateObject(bounds);
            guo.updatePhysics = true;
            AstarPath.active.UpdateGraphs(guo);
        }
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("ReGrow");
        canDie = false;
    }
}