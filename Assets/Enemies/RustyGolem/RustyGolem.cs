using System.Collections;
using UnityEngine;

public class RustyGolem : MonoBehaviour, IGrowableEnemy
{
    /* RUSTY GOLEM
     * Handles all of rusty golem's logic
     * Handles rusty golem's movement
     * Handles rusty golem's
     */

    // ---- INTERFACE ----

    public bool dead = false;
    public bool candie = false;
    public bool isgrown = false;

    public bool CanDie => candie;
    public bool IsGrown => isgrown;
    public bool Dead => dead;

    // ---- BASE COMPONENTS ----
    public float health;

    private Animator animator;
    private Rigidbody2D rb;
    private DruidFrameWork druid;
    private SpriteRenderer spriterenderer;
    private DruidUI UI;
    private DruidGrowFramework growframework;

    // ---- MOVEMENT ----
    public float movespeed = 2f;

    public float pauseTime = 3f;
    public float movedistance = 5f;

    private bool movingright = true;
    private bool isPaused = false;
    private Vector2 startpos;

    // ---- DAMAGE FLASH ----
    public float flashDuration = 0.3f;

    public float flashPeak = 1f;

    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;

    //BouncePad
    [SerializeField] private GameObject collide;

    [SerializeField] private float bounceheight;

    private BoxCollider2D bouncepad;

    /* AWAKE
     * Handles extremely necessary components
     * Handles flash components
     */

    private void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        spriterenderer.material = new Material(spriterenderer.material);

        mpb = new MaterialPropertyBlock();
    }

    /* START
     * Handles player variable
     * Handles components
     */

    private void Start()
    {
        startpos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //find druidreference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            growframework = player.GetComponent<DruidGrowFramework>();
            UI = player.GetComponent<DruidUI>();
            druid = player.GetComponent<DruidFrameWork>();
        }
    }

    /* UPDATE
     * Handles Death
     */

    private void Update()
    {
        animator.SetFloat("XVelo", rb.linearVelocityX);

        //death
        if (health < 1 || health == 0)
        {
            animator.SetTrigger("Death");
            rb.linearVelocityX = 0f;
            rb.linearVelocityY = 0f;
            if (dead == false)
            {
                if (!DruidFrameWork.isTransformed)
                {
                    UI.spirits += 3;
                }
                dead = true;
                growframework.RemoveTether(transform);
            }
        }
    }

    /* FIXED UPDATE
     * Handles Movement
     * Handles Flipping
     */

    private void FixedUpdate()
    {
        if (!dead && !isPaused)
        {
            if (!isgrown)
            {
                float distanceFromStart = transform.position.x - startpos.x;

                if (movingright)
                {
                    spriterenderer.flipX = true;
                    rb.linearVelocityX = movespeed;
                    if (distanceFromStart >= movedistance)
                    {
                        StartCoroutine(PauseAtEnd(false));
                    }
                }
                else
                {
                    spriterenderer.flipX = false;
                    rb.linearVelocityX = -movespeed;
                    if (distanceFromStart <= -movedistance)
                    {
                        StartCoroutine(PauseAtEnd(true));
                    }
                }
            }
        }
        else if (isPaused)
        {
            rb.linearVelocityX = 0f;
        }
    }

    /* FUNCTIONS
     * Handles grow
     * Handles Dying
     * Handles collision with bouncepad if grown
     * Handles taking damage
     * Handles flash
     */

    //grow the enemy
    public void Grow()
    {
        if (!dead)
        {
            if (isgrown == false)
            {
                if (candie == false)
                {
                    isgrown = true;
                    candie = true;
                    StartCoroutine(GrowCycle());
                }
            }
        }
    }

    //ungrow the enemy
    public void Die()
    {
        if (!dead)
        {
            if (isgrown == true)
            {
                if (candie == true)
                {
                    StartCoroutine(DieCycle());
                }
            }
        }
    }

    // ---- DAMAGE ----
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!dead)
        {
            if (collision)
            {
                if (collision.gameObject.CompareTag("SeedBullet")) //checks if touched by bullet
                {
                    Destroy(collision.gameObject);
                    TakeDamage(2f);
                }

                if (collision.gameObject.CompareTag("Player") && isgrown && bouncepad != null)
                {
                    Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocityX, 0f);

                        DruidFrameWork.canjump = false;
                        playerRb.AddForce(Vector2.up * bounceheight, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    public void TakeDamage(float damage) //call to take damage put damage in parameters
    {
        if (!dead)
        {
            health -= damage;
            Flash();
        }
    }

    // ---- FLASH CALL ----
    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    /* COROUTINES
     * Handles damage flash
     * Handles Growing
     * Handles Dying
     * Handles Idle at end of walking
     */

    //flash coroutine
    private IEnumerator FlashCoroutine()
    {
        float timer = 0f;

        spriterenderer.GetPropertyBlock(mpb);

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float t = 1f - (timer / flashDuration);
            float intensity = t * flashPeak;

            mpb.SetFloat("_FlashIntensity", intensity);
            spriterenderer.SetPropertyBlock(mpb);

            yield return null;
        }

        mpb.SetFloat("_FlashIntensity", 0f);
        spriterenderer.SetPropertyBlock(mpb);

        flashRoutine = null; // Clear reference
    }

    private IEnumerator PauseAtEnd(bool turnRight) // pauses at the end of the movement
    {
        isPaused = true;
        rb.linearVelocityX = 0f;

        yield return new WaitForSeconds(pauseTime);

        movingright = turnRight;
        isPaused = false;
    }

    private IEnumerator GrowCycle()
    {
        animator.SetTrigger("grow");
        yield return new WaitForSeconds(0.75f);
        collide.AddComponent<BoxCollider2D>();
        bouncepad = collide.GetComponent<BoxCollider2D>();
        bouncepad.enabled = true;
        bouncepad.usedByEffector = true;
        bouncepad.isTrigger = true;
    }

    private IEnumerator DieCycle()
    {
        candie = false;
        Destroy(bouncepad);
        animator.SetTrigger("die");
        yield return new WaitForSeconds(1f);
        isgrown = false;
    }
}