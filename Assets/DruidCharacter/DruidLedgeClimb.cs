using System.Collections;
using UnityEngine;

public class DruidLedgeClimb : MonoBehaviour
{
    private DruidFrameWork druidFrameWork;
    private SpriteRenderer druidSpriteRenderer;
    private Transform druidTransform;
    private Animator druidAnimator;
    private Rigidbody2D druidRig;
    public static bool isMantled = false;
    [SerializeField] private float ledgeClimbDistance = 0.52f;
    [SerializeField] private float topOffset = 0.5f;
    [SerializeField] private float ledgeClimbOffsetX = 0.5f;
    [SerializeField] private float ledgeClimbOffsetY = 1f;
    [SerializeField] private Vector2 climbSize = new Vector2(0.2f, 0.2f);
    private bool climbing = false;
    private bool upCd = false;
    Vector2 ledgePosition;
    Vector2 climbTargetPos;
    void Start()
    {
        druidRig = GetComponent<Rigidbody2D>();
        druidFrameWork = GetComponent<DruidFrameWork>();  
        druidSpriteRenderer = GetComponent<SpriteRenderer>();
        druidTransform = GetComponent<Transform>();
        druidAnimator = GetComponent<Animator>();
    }

    
    void Update()
    {
        float direction = druidSpriteRenderer.flipX ? -1f : 1f;

        
        Debug.DrawRay(transform.position, new Vector2(direction, 0f) * ledgeClimbDistance, Color.red);
        Debug.DrawRay((Vector2)transform.position + new Vector2(0, topOffset), new Vector2(direction, 0f) * ledgeClimbDistance, Color.green);
        //LEDGE CLIMB
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (druidFrameWork.isGrounded == false && isMantled == false && DruidFrameWork.isTransformed == false)
            {
               
                Vector2 offset = new Vector2(0, topOffset);
                RaycastHit2D bottomHit = Physics2D.Raycast(transform.position, new Vector2(direction, 0f), ledgeClimbDistance, LayerMask.GetMask("Ground"));

                RaycastHit2D topHit = Physics2D.BoxCast((Vector2) druidTransform.position + offset, climbSize, 0f, new Vector2(direction, 0f), ledgeClimbDistance, LayerMask.GetMask("Ground"));

                if (topHit.collider == null && bottomHit.collider != null)
                {
                    isMantled = true;
                    DruidFrameWork.canmove = false;
                    druidRig.bodyType = RigidbodyType2D.Static;
                    Debug.Log("Druid Is Climbing!");
                    DruidFrameWork.canjump = false;

                    //find ledge position

                    druidAnimator.SetTrigger("Mantle");
                    druidAnimator.SetBool("IsMantling", true);
                    ledgePosition = bottomHit.point;
                    climbTargetPos = new Vector2(ledgePosition.x + (direction * ledgeClimbOffsetX), ledgePosition.y + ledgeClimbOffsetY);
                    upCd = true;
                    StartCoroutine(UpCooldown());

                }
            }
        } 

        //MANTLING FRAMEWORK
        if (isMantled == true && upCd == false && climbing == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(LedgeClimb());
                
            } else if (Input.GetKeyDown(KeyCode.S)) 
            {
                druidRig.bodyType = RigidbodyType2D.Dynamic;
                druidAnimator.SetBool("IsMantling", false);
                DruidFrameWork.canjump = true;
                DruidFrameWork.canmove = true;
                isMantled = false;

            }
           
        }
    }

    private IEnumerator UpCooldown()
    {
        yield return new WaitForSeconds(0.15f);
        upCd = false;
    }

    private IEnumerator LedgeClimb()
    {
        climbing = true;
        float climbDuration = 0.2f; 
        Vector2 startPos = druidTransform.position;
        float elapsed = 0f;

        druidAnimator.SetTrigger("Climb");
        yield return new WaitForSeconds(0.1f);

        while (elapsed < climbDuration)
        {
            druidTransform.position = Vector2.Lerp(startPos, climbTargetPos, elapsed / climbDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        druidTransform.position = climbTargetPos;
        yield return new WaitForFixedUpdate();

        RaycastHit2D groundCheck = Physics2D.Raycast(druidTransform.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        if (groundCheck.collider != null)
        {
           
            druidTransform.position += Vector3.up * 0.05f;
        }
        druidRig.bodyType = RigidbodyType2D.Dynamic;
        druidAnimator.SetBool("IsMantling", false);
        DruidFrameWork.canjump = true;
        DruidFrameWork.canmove = true;
        isMantled = false;
        climbing = false;
       
    }


}
