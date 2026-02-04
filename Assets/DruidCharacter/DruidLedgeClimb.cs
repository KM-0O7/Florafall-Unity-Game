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
    [SerializeField] private float cellingCheckDistance = 0.5f;
    [SerializeField] private float ledgeClimbOffsetX = 0.5f;
    [SerializeField] private float ledgeClimbOffsetY = 1f;
    [SerializeField] private Vector2 climbSize = new Vector2(0.2f, 0.2f);

    private float direction;
    public LineRenderer tether;
    private LineRenderer tetherClone;
    private Vector2 ledgePosition;
    private Vector2 climbTargetPos;

    private void Start()
    {
        druidRig = GetComponent<Rigidbody2D>();
        druidFrameWork = GetComponent<DruidFrameWork>();
        druidSpriteRenderer = GetComponent<SpriteRenderer>();
        druidTransform = GetComponent<Transform>();
        druidAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        direction = druidSpriteRenderer.flipX ? -1f : 1f;

        Debug.DrawRay(transform.position, new Vector2(direction, 0f) * ledgeClimbDistance, Color.red);
        Debug.DrawRay((Vector2)transform.position + new Vector2(0, topOffset), new Vector2(direction, 0f) * ledgeClimbDistance, Color.green);

        //LEDGE CLIMB
        if (Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (druidFrameWork.isGrounded == false && isMantled == false && DruidFrameWork.isTransformed == false)
                {
                    LedgeClimbFunction();
                }
            }
        }
    }

    private void LedgeClimbFunction()
    {
        if (isMantled) return;

        Vector2 horizontalOffset = new Vector2(direction * 0.1f, 0f);
        Vector2 offset = new Vector2(0, topOffset);

        RaycastHit2D cellingCheck = Physics2D.Raycast((Vector2)druidTransform.position, Vector2.up, cellingCheckDistance, LayerMask.GetMask("Ground"));
        RaycastHit2D bottomHit = Physics2D.BoxCast((Vector2)transform.position + horizontalOffset, climbSize, 0f, new Vector2(direction, 0f), ledgeClimbDistance, LayerMask.GetMask("Ground"));

        RaycastHit2D topHit = Physics2D.BoxCast((Vector2)druidTransform.position + offset + horizontalOffset, climbSize, 0f, new Vector2(direction, 0f), ledgeClimbDistance, LayerMask.GetMask("Ground"));

        if (topHit.collider == null && bottomHit.collider != null && cellingCheck.collider == null)
        {
            isMantled = true;
            DruidFrameWork.canmove = false;
            druidRig.linearVelocity = Vector2.zero;
            druidRig.gravityScale = 0f;
            druidRig.constraints = RigidbodyConstraints2D.FreezeAll;
            Debug.Log("Druid Is Climbing!");
            DruidFrameWork.canjump = false;

            //find ledge position

            druidAnimator.SetTrigger("Mantle");
            druidAnimator.SetBool("IsMantling", true);
            ledgePosition = bottomHit.point;
            climbTargetPos = new Vector2(ledgePosition.x + (direction * ledgeClimbOffsetX), ledgePosition.y + ledgeClimbOffsetY);

            tetherClone = Instantiate(tether);
            tetherClone.positionCount = 2;
            tetherClone.SetPosition(0, druidTransform.position);
            tetherClone.SetPosition(1, ledgePosition);
            tetherClone.useWorldSpace = true;

            StartCoroutine(LedgeClimb());
        }
    }

    private IEnumerator LedgeClimb()
    {
        float climbDuration = 0.5f;
        Vector2 startPos = druidTransform.position;
        float elapsed = 0f;

        druidAnimator.SetTrigger("Climb");
        yield return new WaitForSeconds(0.25f);

        while (elapsed < climbDuration)
        {
            var lerpEnd = elapsed / climbDuration;
            var quad = 1f - (1f - lerpEnd) * (1f - lerpEnd);

            druidTransform.position = Vector2.Lerp(startPos, climbTargetPos, quad);
            if (tetherClone != null)
            {
                tetherClone.SetPosition(0, druidTransform.position);
                tetherClone.SetPosition(1, ledgePosition);
            }
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
        Destroy(tetherClone.gameObject);
        druidRig.gravityScale = 1f;
        druidRig.constraints = RigidbodyConstraints2D.FreezeRotation;
        druidAnimator.SetBool("IsMantling", false);
        DruidFrameWork.canjump = true;
        DruidFrameWork.canmove = true;
        isMantled = false;
    }
}