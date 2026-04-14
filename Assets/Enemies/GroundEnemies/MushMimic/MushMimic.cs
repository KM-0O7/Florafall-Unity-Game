using System.Collections;
using UnityEngine;

public class MushMimic : MonoBehaviour, IGrowableEnemy, IEnemy
{
    private bool isGrown = false;
    private bool canDie = false;
    [SerializeField] private int cost;
    EnemyDamage damage;
    public bool IsGrown => isGrown;
    public int spiritCost => cost;
    public bool Dead => damage.dead;
    public bool FlyingEnemy => false;
    private bool isLerping = false;
    public bool IsLerping => isLerping;
    public void SetLerp(bool value)
    {
        isLerping = value;
    }
    public bool GroundEnemy => true;

    public bool CanDie => canDie;

    public float flashDuration = 0.3f;

    public float flashPeak = 1f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float jumpHeightCheck = 2f;
    [SerializeField] private float jumpXCheck = 4f; 
    private bool canJump = true;
    private bool isJumping = false;
    GameObject druid;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D enemyRig;

    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private float pauseTime = 3f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private Transform checkPointPos;
    Animator animator;

    private bool movingright = true;
    private bool isPaused = false;
    private Vector2 startpos;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        druid = GameObject.FindGameObjectWithTag("Player");
        damage = GetComponent<EnemyDamage>();
        enemyRig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /* FIXED UPDATE
   * Handles movement
   * Handles pausing and starting at end via coroutine call
   */

    private void FixedUpdate()
    {
        if (!isGrown)
        {
            if (!damage.dead && !isPaused)
            {
                float distanceFromStart = transform.position.x - startpos.x;

                if (movingright)
                {
                    spriteRenderer.flipX = true;
                    enemyRig.linearVelocityX = moveSpeed;
                    if (distanceFromStart >= moveDistance)
                    {
                        StartCoroutine(PauseAtEnd(false));
                    }
                }
                else
                {
                    spriteRenderer.flipX = false;
                    enemyRig.linearVelocityX = -moveSpeed;
                    if (distanceFromStart <= -moveDistance)
                    {
                        StartCoroutine(PauseAtEnd(true));
                    }
                }
            }
            else if (isPaused)
            {
                enemyRig.linearVelocityX = 0f;
            }

            if ((druid.transform.position.y - gameObject.transform.position.y) > jumpHeightCheck && 
                !isJumping && canJump && druid && Mathf.Abs(druid.transform.position.x - gameObject.transform.position.x) < jumpXCheck)
            {
                StartCoroutine(Jump());
            }
        }   
    }


    /* COROUTINES
     * PauseAtEnd handles the mush pausing at the end of their roll
     */

    private IEnumerator Jump()
    {
        isJumping = true;
        canJump = false;

        float direction = movingright ? 1f : -1f;
        enemyRig.AddForce(new Vector2(direction, jumpForce), ForceMode2D.Impulse);
        bool touchingGround = false;
        while (!touchingGround)
        {
            touchingGround = Physics2D.OverlapCircle(checkPointPos.position, 0.2f, LayerMask.GetMask("Ground"));
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        isJumping = false;
        yield return new WaitForSeconds(5);
        canJump = true;
    }

    private IEnumerator PauseAtEnd(bool turnRight) // pauses at the end of the movement
    {
        isPaused = true;
        enemyRig.linearVelocityX = 0f;

        yield return new WaitForSeconds(pauseTime);

        movingright = turnRight;
        isPaused = false;
    }

    public void Grow()
    {
        if (!damage.dead)
        {
            if (IsGrown == false)
            {
                if (canDie == false)
                {
                    isGrown = true;
                    canDie = true;
                    StartCoroutine(GrowCycle());
                }
            }
        }
    }

    public void Die()
    {
        if (!damage.dead)
        {
            if (isGrown == true)
            {
                if (canDie == true)
                {
                    isGrown = false;
                    canDie = false;
                    StartCoroutine(DieCycle());
                }
            }
        }
    }

    private IEnumerator GrowCycle()
    {
        yield return null;
    }

    private IEnumerator DieCycle()
    {
        yield return null;
    }

}
