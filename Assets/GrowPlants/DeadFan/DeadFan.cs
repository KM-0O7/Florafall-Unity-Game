using UnityEngine;
using System.Collections;

public class DeadFan : MonoBehaviour, IGrowablePlant
{
    [SerializeField] private int spirits = 1;
    [SerializeField] private float blowForce = 1.0f;
    private bool fanDb = false;
    public bool candie = false;
    private Transform fanTransform;
    private DruidFrameWork druidFrameWork;
    private Rigidbody2D druidRig;
    [SerializeField] private Vector2 blowSize = new Vector2(0f, 0f);
    public int spiritCost => spirits;
    
    public bool CanDie => candie;
    public bool IsGrown => fanDb;

    private Animator animator;


    void Start()
    {
        fanTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (candie == true)
        {
            RaycastHit2D hit = Physics2D.BoxCast(fanTransform.position, blowSize, 0f, Vector2.up, blowSize.y, LayerMask.GetMask("Player"));

            if (hit)
            {
                druidFrameWork = hit.collider.GetComponent<DruidFrameWork>();

                if (druidFrameWork != null)
                {
                    druidRig = hit.collider.GetComponent<Rigidbody2D>();
                    DruidFrameWork.canjump = false;

                    //Apply force when druid is in the fan
                    ForceMode2D mode = ForceMode2D.Impulse;
                    druidRig.AddForceY(blowForce, mode);
                    
                }
            } 
        }
    }
  
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, Vector2.up * blowSize.y, Color.red);
        Debug.DrawRay(transform.position, Vector2.left * blowSize.x, Color.red);
    }


    public void Grow()
    {
        if (!fanDb)
        {
            if (!candie)
            {
                StartCoroutine(GrowCycle());
            }
        }
    }

    public void Die()
    {
        if (fanDb)
        {
            if (candie)
            {
                StartCoroutine(DieCycle());
            }
        }
    }


    private IEnumerator GrowCycle()
    {
        fanDb = true;
        animator.SetTrigger("Grow");
        yield return new WaitForSeconds(0.3f);
        candie = true;
    }

    private IEnumerator DieCycle()
    {
        candie = false;
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(0.3f);
        fanDb = false;
    }
}
