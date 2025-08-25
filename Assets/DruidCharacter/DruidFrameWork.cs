using System.Collections.Generic;
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
    public int spirits;
    private bool gravityjump = false;

    //tether
    public LineRenderer tether;

    public Transform druidtransform;
    private List<LineRenderer> activeTethers = new List<LineRenderer>();
    private List<Transform> tetherTargets = new List<Transform>();

    //jump parameters
    [SerializeField] private Transform groundCheck;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
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

        if (canjump == true)
        {
            animator.SetFloat("XVelo", speedx);
        }
        else if (!canjump)

        {
            animator.SetFloat("XVelo", 0f);
        }

        if (speedx > 0f)
        {
            druidspriterender.flipX = false;
        }
        else if (speedx < 0f)
        {
            druidspriterender.flipX = true;
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

        //ResetJump

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && druidrb.linearVelocityY <= 0)
        {
            canjump = true;
        }

        //updatetethers via list
        for (int i = 0; i < activeTethers.Count; i++)
        {
            if (activeTethers[i] != null && tetherTargets[i] != null)
            {
                activeTethers[i].SetPosition(0, druidtransform.position);
                activeTethers[i].SetPosition(1, tetherTargets[i].position);
            }
        }

        //plantframework

        if (Input.GetMouseButtonDown(0))
        {
            if (spirits > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                LayerMask plantLayer = LayerMask.GetMask("GrowPlants");
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, plantLayer);

                if (hit.collider != null)
                {
                    Transform plantTransform = hit.collider.transform;

                    //Mushroom
                    if (hit.collider.CompareTag("Mushroom"))
                    {
                        Debug.Log("MushroomGrowing");
                        MushroomPlant mush = hit.collider.GetComponent<MushroomPlant>();

                        if (mush.mushdb == false)
                        {
                            growplant(plantTransform);
                            mush.GrowMush();
                        }
                        else if (mush.candie == true)
                        {
                            animator.SetTrigger("Grow");
                            Debug.Log("Dying");
                            mush.die();
                            spirits++;
                        }
                    }
                    //GlowRoot
                    else if (hit.collider.CompareTag("GlowRoot"))
                    {
                        Debug.Log("GlowRootGrowing");
                        GlowRootPlant root = hit.collider.GetComponent<GlowRootPlant>();

                        if (root.glowdb == false)
                        {
                            growplant(plantTransform);
                            root.GrowGlowRoot();
                        }
                        else if (root.candie == true)
                        {
                            animator.SetTrigger("Grow");
                            Debug.Log("Dying");
                            root.die();
                            spirits++;
                        }
                    }
                    //SeedCannon
                    else if (hit.collider.CompareTag("SeedCannon"))
                    {
                        Debug.Log("SeedCannonGrowing");
                        SeedCannon cannon = hit.collider.GetComponent<SeedCannon>();

                        if (cannon.cannondb == false)
                        {
                            growplant(plantTransform);
                            cannon.GrowGlowRoot();
                        }
                        else if (cannon.candie == true)
                        {
                            animator.SetTrigger("Grow");
                            Debug.Log("Dying");
                            cannon.die();
                            spirits++;
                        }
                    }
                }
            }
        }
    }

    //removes a spirit and makes the druid do her grow animation and attaches a tether
    private void growplant(Transform plantTransform)
    {
        LineRenderer tetherclone = Instantiate(tether);
        tetherclone.positionCount = 2;
        tetherclone.SetPosition(0, druidtransform.position);
        tetherclone.SetPosition(1, plantTransform.position);
        tetherclone.useWorldSpace = true;
        activeTethers.Add(tetherclone);
        tetherTargets.Add(plantTransform);

        spirits -= 1;
        animator.SetTrigger("Grow");
    }
}