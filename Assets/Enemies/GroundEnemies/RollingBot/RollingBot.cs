using System.Collections;
using UnityEngine;

public class RollingBot : MonoBehaviour, IEnemy
{
    /* ROLLING BOT
     * Handles the movement of a rolling bot
     * Handles activation of rolling bot
     * Handles damage and flash
     */

    //---- INTERFACE ----
    public bool FlyingEnemy => false;
    public bool GroundEnemy => true;
  

    private bool isLerping = false; 
    public bool IsLerping => isLerping;
    public void SetLerp(bool value)
    {
        isLerping = value;
    }

    //---- BASE COMPONENTS ----
    private Transform druidTransform;

    private Transform rollingBotTransform;
    private SpriteRenderer spriterenderer;
    private Animator animator;
    private Rigidbody2D rb;
    private EnemyDamage damage;

    //---- ACTIVATION ----
    private bool activated = false;

    private bool isActivating = false;

    //---- MOVEMENT ----
    [SerializeField] private float moveSpeed = 2f;

    [SerializeField] private float pauseTime = 3f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float activationDistance = 15f;

    private bool movingright = true;
    private bool isPaused = false;
    private Vector2 startpos;
    public bool Dead => damage.dead;

    /* START
     * Handles components
     * Handles getting player
     * Handles flash components
     */

    private void Start()
    {
        damage = GetComponent<EnemyDamage>();
        startpos = transform.position;
        rollingBotTransform = gameObject.GetComponent<Transform>();
        spriterenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
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
            if (!damage.dead && !isPaused)
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
     * DrawGizmos shows the activation range when selecting the rolling bot
     */

    //shows activation range when selected in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }

    /* COROUTINES
     * ActivationCoroutine handles the activation after anims
     * PauseAtEnd handles the rolling bot pausing at the end of their roll
     */

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