using UnityEngine;

public class ExplosionShroom : MonoBehaviour, IGrowablePlant
{
    public bool explosiondb = false;
    public bool candie = false;
    private Animator animator;
    [SerializeField] private DruidFrameWork druid;
    public bool waterGrown = false;
    public bool WaterGrown => waterGrown;
    private bool canGrow = true;
    public bool CanGrow => canGrow;
    public void setWaterGrow(bool value)
    {
        waterGrown = value;
    }
    public bool IsGrown => explosiondb;
    public bool CanDie => candie;
    [SerializeField] private GameObject shroom;
    private int spirits = 1;
    public int spiritCost => spirits;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Grow()
    {
        if (explosiondb == false)
        {
            if (candie == false && canGrow)
            {
                explosiondb = true;
                candie = true;
                animator.SetTrigger("grow");
            }
        }
    }

    public void Die()
    {
        Destroy(shroom);
    }
}