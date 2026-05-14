using System.Collections;
using UnityEngine;
using Pathfinding;

public class IceWall : MonoBehaviour, IGrowablePlant
{
    [SerializeField] private int growthCost = 2;
    private bool isGrown = false;
    private bool canDie = false;
    public bool CanDie => canDie;
    public bool waterGrown = false;
    public bool WaterGrown => waterGrown;
    private bool canGrow = true;
    public bool CanGrow => canGrow;
    public void setWaterGrow(bool value)
    {
        waterGrown = value;
    }
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
        if (!canDie && !isGrown && canGrow) StartCoroutine(GrowCycle());
    }

    public void Die()
    {
        if (canDie && isGrown && canGrow) StartCoroutine(DieCycle());
    }

    private IEnumerator GrowCycle()
    {
        isGrown = true;
        animator.SetTrigger("Grow");
        canGrow = false;
        yield return new WaitForSeconds(0.4f);
        canGrow = true;
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
        canGrow = false;
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
        canGrow = true;
        animator.SetTrigger("ReGrow");
        canDie = false;
    }
}