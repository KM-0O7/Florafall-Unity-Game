using System.Collections;
using UnityEngine;

public class RustyGolem : MonoBehaviour, IGrowableEnemy
{
    public bool candie = false;
    public bool isgrown = false;
    public float health;
    public bool CanDie => candie;
    public bool IsGrown => isgrown;
    private Animator animator;
    private Rigidbody2D rb;
    public bool dead = false;
    private DruidFrameWork druid;
    private SpriteRenderer spriterenderer;

    //movement
    public float movespeed = 2f;

    public float pauseTime = 3f;
    public float movedistance = 5f;
    private Vector2 startpos;
    private bool movingright = true;
    private bool isPaused = false;
    public bool Dead => dead;

    //DamageFlash
    public float flashDuration = 0.3f;

    public float flashPeak = 1f;
    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;

    //BouncePad
    [SerializeField] private GameObject collide;

    [SerializeField] private float bounceheight;
    private BoxCollider2D bouncepad;

    private void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        spriterenderer.material = new Material(spriterenderer.material);

        mpb = new MaterialPropertyBlock();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        startpos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //find druidreference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            druid = player.GetComponent<DruidFrameWork>();
        }
    }

    // Update is called once per frame
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
                druid.spirits += 3;
                dead = true;
                druid.RemoveTether(transform);
            }
        }
    }

    //Movement
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

    private IEnumerator PauseAtEnd(bool turnRight) // pauses at the end of the movement
    {
        isPaused = true;
        rb.linearVelocityX = 0f;

        yield return new WaitForSeconds(pauseTime);

        movingright = turnRight;
        isPaused = false;
    }

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

    //damage

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

    private void TakeDamage(float damage) //call to take damage put damage in parameters
    {
        if (!dead)
        {
            health -= damage;
            Flash();
        }
    }

    //flash call
    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashCoroutine());
    }

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
}