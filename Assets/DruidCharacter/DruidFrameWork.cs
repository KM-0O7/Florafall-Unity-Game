using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DruidFrameWork : MonoBehaviour
{

    /* DRUIDFRAMEWORK
     * This script handles movement, transformations, and jumping for the main character
     * Includes functions for attack
     * Includes coroutines for freezeframes and transformations
     * Connects to other druid scripts
     */

    /* VARIABLES
     * Handles all changing parts in druidframework
     * Handles UI
     * Handles Components
     * Handles Jump
     * etc
     */
    
    // ---- MOVEMENT ----
    private Rigidbody2D druidrb;
    private Animator animator; 
    private SpriteRenderer druidspriterender;
    private BoxCollider2D boxcollider;
    private float speedx;

    public float druidspeed;
    public static bool canjump = true;
    public static bool canmove = true;
    public Transform druidtransform;

    // ---- CUSTOM JUMP PHYSICS ----
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
   
    private bool isJumping;
    private bool isGrounded;
    private bool gravityjump = false;
    private float jumpheight = 7.5f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float variableJumpMultiplier = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float druidJumpHeight;
    [SerializeField] private float bearJumpHeight;

    // ---- CUSTOM CURSOR ----
    public Texture2D cursorTexture;
    private Vector2 cursorHotspot;

    // ---- UI ----
    DruidUI UI;
    [SerializeField] private Animator UIwalker;

    // ---- TRANSFORMATIONS ----
    private bool isAttacking = false;
    private bool damagecd = false;
    private bool istransforming = false;
    private bool transformcd = false;

    public static bool Transitioning = false;
    public static bool bearattackcd = false;
    public static bool isTransformed = false;
 
    [SerializeField] private float biteLength = 2.5f;

    /* START
     * Handles all components
     * Handles custom cursor
     */
    private void Start()
    {
        // ---- BASIC COMPONENTS ----
        UI = GetComponent<DruidUI>();
        boxcollider = GetComponent<BoxCollider2D>();
        druidrb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        druidspriterender = GetComponent<SpriteRenderer>();

        // ---- CURSOR ----
        cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

   /* FIXEDUPDATE
    * Handles all left to right movement logic
    * Handles coyote time
    * Handles UI transitions with the druid and bear in the circle wipe
    * Handles flip x of druid sprite 
    * Handles animations for movement such as jump and walking
    */

    private void FixedUpdate()
    {
        // ---- WALKING ----
        animator.SetFloat("YVelo", druidrb.linearVelocityY);

        
        if (!isAttacking) //checks if not attacking
        {
            speedx = Input.GetAxisRaw("Horizontal");
            druidrb.linearVelocityX = speedx * druidspeed; //sets velo to your movement direction times speed

            if (isGrounded)
            {
                animator.SetFloat("XVelo", Mathf.Abs(speedx));
            }
            else
            {
                animator.SetFloat("XVelo", 0f);
            }
        }
        else
        {
            animator.SetFloat("XVelo", 0f);
        }


        // ---- FLIP X LOGIC AND UI LOGIC ----
        if (speedx > 0f) //forwards
        {
            druidspriterender.flipX = false;

            if (!Transitioning)
            {
                UIwalker.SetBool("Backwards", false);
            }
        }


        else if (speedx < 0f) //backwards
        {
            druidspriterender.flipX = true;
            if (!Transitioning)
            {
                UIwalker.SetBool("Backwards", true);
            }
        }

        // ---- JUMP ANIMATIONS ----
        if (druidrb.linearVelocityY > 0.5f)
        {
            animator.SetTrigger("Jump");
        }

        // ---- COYOTE TIME & RESET JUMP ----
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && druidrb.linearVelocityY <= 0.1f)
        {
            coyoteTimeCounter = coyoteTime;
            canjump = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            canjump = false;
        }
    }

    /* UPDATE
     * Update handles most jump logic and key inputs
     * Handles jump buffer
     * Handles Q to transform
     * Handles jump input
     */


    private void Update()
    {
        // ---- JUMP ----

        // ---- BUFFER ----
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isAttacking && !istransforming && canmove)
        {
            druidrb.linearVelocityY = jumpheight;
            isJumping = true;
            jumpBufferCounter = 0f;
        }

        // ---- VARIABLE JUMP HEIGHT ----
        if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            if (druidrb.linearVelocityY > 0f)
            {
                druidrb.linearVelocityY *= variableJumpMultiplier;
            }

            isJumping = false;
        }


        // ---- FASTER JUMP FALL ----
        if (!istransforming)
        {
            if (canjump == false)
            {
                if (gravityjump)
                {
                    druidrb.gravityScale += 0.5f; //add 0.5 to gravity when falling so it feels less floaty when jumping
                    gravityjump = false;
                }
            }
            else
            {
                if (!gravityjump)
                {
                    gravityjump = true;
                    druidrb.gravityScale = 1f; //set back to normal gravity
                }
            }
        }


        // ---- TRANSFORMATIONS INPUT ----
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isTransformed)
            {
                if (UI.spirits == 5)
                {
                    if (!istransforming)
                    {
                        if (!Transitioning)
                        {
                            StartCoroutine(TransformIntoAnimal("Bear"));
                        }
                    }
                }
            }
            else
            {
                if (!istransforming)
                {
                    if (!Transitioning)
                    {
                        StartCoroutine(TransformIntoDruid());
                    }
                }
            }
        }
    }

    /* FUNCTIONS
     * Beartattack will trigger the bear to attack by calling a coroutine
     * Changecollidersize will change the druids collider to a new size and a new offset useful for transformations
     */

    public void BearAttack() //call this to attack while bear
    {
        StartCoroutine(attack());
    }
   
    // ---- COLLIDER SIZE ----
    private void ChangeColliderSize(Vector2 newsize, Vector2 newoffset)
    {
        boxcollider.offset = newoffset;
        boxcollider.size = newsize;
    }


    /* COROUTINES
     * Calling attack will trigger the bearattack animation and raycast a hit
     * calling freezeframe will freeze the game for a set duration as stated in parameters
     * calling transform into animal will trigger you to transform into an animal as stated in parameters
     * calling transform into druid will trigger you to transform back into a druid
     */


    private IEnumerator attack() //call to attack as bear
    {
        bearattackcd = true;
        isAttacking = true;
        canjump = false;

        animator.SetTrigger("BearBite");
        druidrb.linearVelocityX = 0f;

        yield return new WaitForSeconds(0.7f);

        float direction = druidspriterender.flipX ? -1f : 1f; //checks which way facing
        RaycastHit2D hit = Physics2D.Raycast(druidtransform.position, new Vector2(direction, 0f), biteLength, LayerMask.GetMask("GrowEnemy"));
        if (hit)
        {
            Debug.Log("RaycastConnected");
            if (!damagecd)
            {
                if (hit.collider != null)
                {
                    IGrowableEnemy enemy = hit.collider.GetComponent<IGrowableEnemy>();
                    if (enemy != null)
                    {
                        if (!enemy.Dead)
                        {
                            Debug.Log("Hit");
                            damagecd = true;
                            enemy.TakeDamage(3f);
                            yield return StartCoroutine(FreezeFrame(0.25f));
                            float burstSpeed = 4f; // Apply burst of speed in facing direction

                            druidrb.AddForce(new Vector2(burstSpeed * -direction, 0f), ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        damagecd = false;
        isAttacking = false;
        canjump = true;
        yield return new WaitForSeconds(3f); // Cooldown
        bearattackcd = false;
    }

    private IEnumerator FreezeFrame(float duration) //freezeframe
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration); // unaffected by timeScale
        Time.timeScale = originalTimeScale;
    }

    private IEnumerator TransformIntoAnimal(string animal) //call to transform into animal as stated in parameters
    {
        if (animal == "Bear")
        {
            if (transformcd == false)
            {
                
                
                UI.spirits = 0;
                druidrb.linearVelocityX = 0f;
                druidrb.linearVelocityY = 0f;
                druidrb.gravityScale = 0f;

                isAttacking = true;
                animator.SetTrigger("TransformBear");
                istransforming = true;
                canjump = false;

                yield return new WaitForSeconds(0.4f);//after anim plays
                UIwalker.SetBool("Bear", true);
                animator.SetBool("Bear", true);
                istransforming = false;
                isAttacking = false;
                druidrb.gravityScale = 1f;
                canjump = true;
                groundCheck.localPosition -= new Vector3(0, 0.17f, 0);
                jumpheight = bearJumpHeight;
                ChangeColliderSize(new Vector2(0.9f, 0.43f), new Vector2(-0.05f, -0.42f));
                isTransformed = true;
                animator.SetFloat("XVelo", speedx);
            }
        }
    }

    private IEnumerator TransformIntoDruid() //call to transform back into a druid
    {
        transformcd = true;
        canjump = false;
        UIwalker.SetBool("Bear", false);
        UI.spirits = 5;
        druidrb.linearVelocityX = 0f;
        druidrb.linearVelocityY = 0f;
        druidrb.gravityScale = 0f;

        istransforming = true;
        animator.SetBool("IsTransforming", true);
        isAttacking = true;
        jumpheight = druidJumpHeight;
        animator.SetBool("Bear", false);

        yield return new WaitForSeconds(0.3f);//after anim plays
        animator.SetBool("IsTransforming", false);
        istransforming = false;
        isAttacking = false;
        druidrb.gravityScale = 1f;
        canjump = true;
        groundCheck.localPosition += new Vector3(0, 0.17f, 0);
        ChangeColliderSize(new Vector2(0.7f, 0.53f), new Vector2(0f, -0.2f));
        isTransformed = false;
        animator.SetFloat("XVelo", speedx);

        yield return new WaitForSeconds(1f);
        transformcd = false;
    }
}