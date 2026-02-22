using System.Collections;
using UnityEngine;

public class IcyGolem : MonoBehaviour, IGrowableEnemy, IDamageAble, IEnemy
{
    /* ICY GOLEM
      * Handles all of icy golem's logic
      * Handles icy golem's movement
      * Handles icy golem's fan
      */

    // ---- INTERFACE ----

    public bool dead = false;
    public bool candie = false;
    public bool isgrown = false;
    public bool FlyingEnemy => false;
    public bool GroundEnemy => true;

    private bool isLerping = false;
    public bool IsLerping => isLerping;
    public void SetLerp(bool value)
    {
        isLerping = value;
    }

    public bool CanDie => candie;
    public bool IsGrown => isgrown;
    public bool Dead => dead;
    public int spiritCost => 3;

    // ---- BASE COMPONENTS ----
    public float health;

    private Animator animator;
    private Rigidbody2D rb;
    private DruidFrameWork druid;
    private SpriteRenderer spriterenderer;
    private DruidUI UI;
    private DruidGrowFramework growframework;
    private Rigidbody2D druidRig;

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
    private bool hitImmune = false;

    // ---- FAN ----
    [SerializeField] private float blowForce = 0.5f;
    [SerializeField] private Vector2 blowSize = new Vector2(0f, 0f);
    private ParticleSystem fanParticle;
    private ParticleSystem.EmissionModule emission;

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
        fanParticle = GetComponent<ParticleSystem>();
        emission = fanParticle.emission;
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
            if (!candie)
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

        if (emission.enabled && !dead)
        {
            RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + new Vector2(0f, blowSize.y / 2), blowSize, 0f, Vector2.up, blowSize.y, LayerMask.GetMask("Player"));
            float ceilingY = transform.position.y + blowSize.y - 0.1f;

            if (hit)
            {
                druid = hit.collider.GetComponent<DruidFrameWork>();

                if (druid != null)
                {
                    druidRig = hit.collider.GetComponent<Rigidbody2D>();
                    DruidFrameWork.canjump = false;
                    Debug.Log("Adding Force");
                    ForceMode2D mode = ForceMode2D.Impulse;
                    if (druid.druidtransform.position.y < ceilingY)
                    {
                        druidRig.AddForceY(blowForce, mode);
                    }
                    else
                    {
                        druidRig.linearVelocity = new Vector2(druidRig.linearVelocity.x, Mathf.Lerp(druidRig.linearVelocity.y, 0f, 0.1f));
                    }
                }
            }
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
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, Vector2.up * blowSize.y, Color.red);
        Debug.DrawRay(transform.position, Vector2.left * blowSize.x, Color.red);
    }
 
    public void TakeDamage(float damage) //call to take damage put damage in parameters
    {
        if (!dead)
        {
            if (!hitImmune)
            {
                hitImmune = true;
                StartCoroutine(HitImmuneCoroutine(0.5f));
                health -= damage;
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

    /* COROUTINES
     * Handles damage flash
     * Handles Growing
     * Handles Dying
     * Handles Idle at end of walking
     * Handles HitImmunity
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

    private IEnumerator HitImmuneCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        hitImmune = false;
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
        animator.SetTrigger("Grow");
       
        rb.linearVelocityX = 0f;
        candie = true;
      
        yield return new WaitForSeconds(0.4f);
        isgrown = true;
        emission.enabled = true;
       
    }
    private IEnumerator DieCycle()
    {
     
        rb.linearVelocityX = 0f;
        animator.SetTrigger("Die");
        isgrown = false;
        emission.enabled = false;
        yield return new WaitForSeconds(1.6f);
        candie = false; 
    }
}
