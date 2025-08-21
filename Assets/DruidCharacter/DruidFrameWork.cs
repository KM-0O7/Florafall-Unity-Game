using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DruidFrameWork : MonoBehaviour
{
    private Rigidbody2D druidrb;
    private Animator animator;
    public float druidspeed;
    private float speedx;
    private SpriteRenderer druidspriterender;
    public static bool canjump = true;

    public Texture2D cursorTexture;
    private Vector2 cursorHotspot;

    //UI/spirits
    public Image[] spiritimages;

    public float spiritregendelay = 3f;
    public Sprite fullSpirit;
    public Sprite emptySpirit;
    public int maxSpirits;
    private bool recentlygrew = false;
    public int spirits;
    private bool gravityjump = false;

    //jump parameters
    [SerializeField] private Transform groundCheck;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //starts the regen loop
        StartCoroutine(SpiritLoop());

        //components
        druidrb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        druidspriterender = GetComponent<SpriteRenderer>();

        //cursor
        cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    // Update is called once per frame
    private void Update()
    {
        animator.SetFloat("YVelo", druidrb.linearVelocityY);

        //Movement
        speedx = Input.GetAxisRaw("Horizontal");
        druidrb.linearVelocityX = speedx * druidspeed;

        if (speedx > 0f)
        {
            animator.SetFloat("XVelo", 1f);
            druidspriterender.flipX = false;
        }
        else if (speedx < 0f)
        {
            animator.SetFloat("XVelo", 1f);
            druidspriterender.flipX = true;
        }
        else if (speedx == 0f)
        {
            animator.SetFloat("XVelo", 0f);
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canjump == true)
            {
                if (druidrb.linearVelocityY > -0.1f)
                {
                    canjump = false;
                    druidrb.linearVelocityY += 7;
                    animator.SetTrigger("Jump");
                }
            }
        }

        //fasterjumpfall
        if (canjump == false)
        {
            if (gravityjump)
            {
                druidrb.gravityScale += 0.5f;
                gravityjump = false;
            }
        }
        else
        {
            if (!gravityjump)
            {
                gravityjump = true;
                druidrb.gravityScale = 1f;
            }
        }

        //spiritUI
        for (int i = 0; i < spiritimages.Length; i++)
        {
            if (i < spirits)
            {
                spiritimages[i].sprite = fullSpirit;
            }
            else
            {
                spiritimages[i].sprite = emptySpirit;
            }

            if (i < maxSpirits)
            {
                spiritimages[i].enabled = true;
            }
            else
            {
                spiritimages[i].enabled = false;
            }
        }

        if (druidrb.linearVelocityY < 0f)
        {
            animator.SetFloat("XVelo", 0f);
        }

        //ResetJump

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && druidrb.linearVelocityY <= 0)
        {
            canjump = true;
        }

        //plantframework

        if (Input.GetMouseButtonDown(0))
        {
            if (spirits > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    //Mushroom
                    if (hit.collider.CompareTag("Mushroom"))
                    {
                        Debug.Log("MushroomGrowing");
                        MushroomPlant mush = hit.collider.GetComponent<MushroomPlant>();

                        if (mush.mushdb == false)
                        {
                            StartCoroutine(growplant());
                            mush.GrowMush();
                        }
                    }
                    //GlowRoot
                    else if (hit.collider.CompareTag("GlowRoot"))
                    {
                        Debug.Log("GlowRootGrowing");
                        GlowRootPlant root = hit.collider.GetComponent<GlowRootPlant>();

                        if (root.glowdb == false)
                        {
                            StartCoroutine(growplant());
                            root.GrowGlowRoot();
                        }
                    }
                    //SeedCannon
                    else if (hit.collider.CompareTag("SeedCannon"))
                    {
                        Debug.Log("SeedCannonGrowing");
                        SeedCannon cannon = hit.collider.GetComponent<SeedCannon>();

                        if (cannon.cannondb == false)
                        {
                            StartCoroutine(growplant());
                            cannon.GrowGlowRoot();
                        }
                    }
                }
            }
        }
    }

    //removes a spirit and makes the druid do her grow animation
    private IEnumerator growplant()
    {
        recentlygrew = true;
        spirits -= 1;
        animator.SetTrigger("Grow");
        yield return new WaitForSeconds(1f);
        recentlygrew = false;
    }

    //spiritloop
    private IEnumerator SpiritLoop()
    {
        while (true) // runs forever
        {
            if (spirits < maxSpirits)
            {
                yield return new WaitForSeconds(spiritregendelay);
                if (!recentlygrew)
                {
                    spirits += 1;
                }
            }
            else
            {
                yield return null; // wait one frame, check again
            }
        }
    }
}