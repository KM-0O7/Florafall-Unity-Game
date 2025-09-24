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
    DruidFrameWork druid;
    private SpriteRenderer spriterenderer;

    //movement
    public float movespeed = 2f;

    public float pauseTime = 3f;
    public float movedistance = 5f;
    private Vector2 startpos;
    private bool movingright = true;
    private bool isPaused = false;
    public bool Dead => dead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
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

    private IEnumerator PauseAtEnd(bool turnRight)
    {
        isPaused = true;
        rb.linearVelocityX = 0f;

        yield return new WaitForSeconds(pauseTime);

        movingright = turnRight;
        isPaused = false;
    }

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

        yield return null;
    }

    private IEnumerator DieCycle()
    {
        candie = false;

        animator.SetTrigger("die");
        yield return new WaitForSeconds(1f);
        isgrown = false;
    }
}