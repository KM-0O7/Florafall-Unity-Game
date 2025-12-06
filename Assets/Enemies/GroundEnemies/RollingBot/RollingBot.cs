using System.Collections;
using UnityEngine;

public class RollingBot : MonoBehaviour, IDamageAble
{
    /* ROLLING BOT
     * Handles the movement of a rolling bot
     * Handles activation of rolling bot
     * Handles damage and flash
     */

    //---- INTERFACE ----
    private bool dead = false;

    public bool Dead => dead;

    //---- BASE COMPONENTS ----
    private Transform druidTransform;

    private Transform rollingBotTransform;
    private SpriteRenderer spriterenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;
    private Animator animator;
    private Rigidbody2D rb;

    //---- DAMAGE ----
    private bool hitImmune = false;

    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float flashPeak = 1f;

    //---- ACTIVATION ----
    private bool activated = false;

    private bool isActivating = false;

    //---- MOVEMENT ----
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private float pauseTime = 3f;
    [SerializeField] private float health = 4f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float activationDistance = 15f;

    private bool movingright = true;
    private bool isPaused = false;
    private Vector2 startpos;

    /* START
     * Handles components
     * Handles getting player
     * Handles flash components
     */

    private void Start()
    {
        startpos = transform.position;
        rollingBotTransform = gameObject.GetComponent<Transform>();
        spriterenderer = gameObject.GetComponent<SpriteRenderer>();
        spriterenderer.material = new Material(spriterenderer.material);
        animator = gameObject.GetComponent<Animator>();
        mpb = new MaterialPropertyBlock();
        rb = gameObject.GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            druidTransform = player.GetComponent<Transform>();
        }
    }

    /* FIXED UPDATE
     * Handles movement
     * Handles pausing and starting at end via coroutine call
     */

    private void FixedUpdate()
    {
        if (activated)
        {
            if (!dead && !isPaused)
            {
                float distanceFromStart = transform.position.x - startpos.x;

                if (movingright)
                {
                    spriterenderer.flipX = true;
                    rb.linearVelocityX = moveSpeed;
                    if (distanceFromStart >= moveDistance)
                    {
                        StartCoroutine(PauseAtEnd(false));
                    }
                }
                else
                {
                    spriterenderer.flipX = false;
                    rb.linearVelocityX = -moveSpeed;
                    if (distanceFromStart <= -moveDistance)
                    {
                        StartCoroutine(PauseAtEnd(true));
                    }
                }
            }
            else if (isPaused)
            {
                rb.linearVelocityX = 0f;
            }
        }
    }

    /* UPDATE
     * Handles movement animations
     * Handles death
     * Handles death animations
     * Handles activation
     */

    private void Update()
    {
        animator.SetFloat("XVelo", rb.linearVelocityX);

        //---- DEATH ----
        if (health < 1 || health == 0)
        {
            animator.SetTrigger("Death");
            rb.linearVelocityX = 0f;
            rb.linearVelocityY = 0f;
            if (dead == false)
            {
                dead = true;
            }
        }

        // ---- ACTIVATION ----
        if (activated == false && isActivating == false)
        {
            if (druidTransform != null && rollingBotTransform.transform != null)
            {
                float distance = Vector2.Distance(druidTransform.position, rollingBotTransform.position);

                if (distance <= activationDistance)
                {
                    isActivating = true;
                    StartCoroutine(ActivationCoroutine());
                }
            }
        }
    }

    /* FUNCTIONS
     * Take damage is required by interface
     * Calling flash makes the sprite flash depending on variable set
     * DrawGizmos shows the activation range when selecting the rolling bot
     */

    public void TakeDamage(float damage) //call to take damage put damage in parameters
    {
        if (!dead)
        {
            if (!hitImmune)
            {
                hitImmune = true;
                health -= damage;
                StartCoroutine(HitImmuneCoroutine(0.5f));
                Flash();
            }
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

    //shows activation range when selected in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }

    /* COROUTINES
     * FlashCoroutine handles flashing when called
     * HitImmuneCoroutine handles disabling hitimmunity after time set in parameters
     * ActivationCoroutine handles the activation after anims
     * PauseAtEnd handles the rolling bot pausing at the end of their roll
     */

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

    private IEnumerator HitImmuneCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        hitImmune = false;
    }

    private IEnumerator ActivationCoroutine()
    {
        Debug.Log(gameObject.name + " Is activating!");
        animator.SetTrigger("Activate");

        yield return new WaitForSeconds(1f);
        isActivating = false;
        activated = true;
    }

    private IEnumerator PauseAtEnd(bool turnRight) // pauses at the end of the movement
    {
        isPaused = true;
        rb.linearVelocityX = 0f;

        yield return new WaitForSeconds(pauseTime);

        movingright = turnRight;
        isPaused = false;
    }
}