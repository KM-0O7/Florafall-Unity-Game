using System.Collections;
using UnityEngine;

public class Sprinkler : MonoBehaviour, IGrowablePlant
{
    [SerializeField] private int growthCost = 5;
    private bool isGrown = false;
    private bool canDie = false;
    public bool CanDie => canDie;
    public bool IsGrown => isGrown;
    public bool waterGrown = false;
    [SerializeField] private ParticleSystem waterLittleDrop;
    [SerializeField] private ParticleSystem showerParticle;
    [SerializeField] private float boxCheckWidth = 3f;

    public void setWaterGrow(bool value)
    {
        waterGrown = value;
    }

    public bool WaterGrown => waterGrown;
    public int spiritCost => growthCost;
    private Animator animator;
    [SerializeField] private Transform waterDropPos;
    private DruidGrowFramework DGF;

    private void Start()
    {
        animator = GetComponent<Animator>();
        DGF = GameObject.FindGameObjectWithTag("Player").GetComponent<DruidGrowFramework>();
    }

    private void Update()
    {
        if (isGrown)
        {
            RaycastHit2D growChecker = Physics2D.Raycast(transform.position, Vector2.down, 10, LayerMask.GetMask("Ground"));

            if (!growChecker) return;

            float height = Mathf.Abs(transform.position.y - growChecker.point.y);
            Vector2 boxSize = new Vector2(boxCheckWidth, height);

            RaycastHit2D boxCheck = Physics2D.BoxCast(transform.position, boxSize, 0f, Vector2.down, 0f, LayerMask.GetMask("GrowPlants"));

            if (!boxCheck) return;

            IGrowablePlant growInterface = boxCheck.collider.GetComponent<IGrowablePlant>();

            if (growInterface != null && growInterface != gameObject.GetComponent<IGrowablePlant>())
            {
                if (!growInterface.IsGrown)
                {
                    growInterface.setWaterGrow(true);
                    growInterface.Grow();
                }
            }
        }
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
        animator.SetTrigger("Grow");
        canDie = true;
        var emission1 = waterLittleDrop.emission;
        var emission2 = showerParticle.emission;
        emission1.enabled = false;
        yield return new WaitForSeconds(1f);
        emission2.enabled = true;
        isGrown = true;
    }

    private IEnumerator DieCycle()
    {
        animator.SetTrigger("Die");
        isGrown = false;
        var emission1 = waterLittleDrop.emission;
        var emission2 = showerParticle.emission;
        emission2.enabled = false;
        yield return new WaitForSeconds(1f);
        emission1.enabled = true;
        canDie = false;
    }
}