using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DruidFrameWork : MonoBehaviour
{
    //movement
    private Rigidbody2D druidrb;

    private Animator animator;
    public float druidspeed;
    private float speedx;
    private SpriteRenderer druidspriterender;
    public static bool canjump = true;
    public static bool canmove = true;

    //cursor
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

    public float maxTetherDistance;
    public Transform druidtransform;
    private List<LineRenderer> activeTethers = new List<LineRenderer>();
    private List<Transform> tetherTargets = new List<Transform>();

    //jump parameters
    [SerializeField] private Transform groundCheck;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.2f;

    //transformations
    private BoxCollider2D boxcollider;

    private bool isTransformed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //components
        boxcollider = GetComponent<BoxCollider2D>();
        druidrb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        druidspriterender = GetComponent<SpriteRenderer>();

        //cursor
        cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }

    //movement
    private void FixedUpdate()
    {
        animator.SetFloat("YVelo", druidrb.linearVelocityY);

        //Walking
        speedx = Input.GetAxisRaw("Horizontal");

        if (canmove) //checks if you just entered a scene
        {
            druidrb.linearVelocityX = speedx * druidspeed;
        }
        else //sets velo to 0 after u enter a scene
        {
            druidrb.linearVelocityX = 0;
        }

        //anims
        if (canjump == true)
        {
            if (druidrb.linearVelocityY > -0.1f)
            {
                animator.SetFloat("XVelo", speedx);
            }
            else
            {
                animator.SetFloat("XVelo", 0f);
            }
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

        if (druidrb.linearVelocityY > 0.5f)
        {
            animator.SetTrigger("Jump");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canjump == true)
            {
                if (druidrb.linearVelocityY > -0.1f)
                {
                    canjump = false;
                    druidrb.linearVelocityY += 7;
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

        //ResetJump

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && druidrb.linearVelocityY <= 0)
        {
            canjump = true;
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

        //transformations

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isTransformed)
            {
                groundCheck.localPosition -= new Vector3(0, 0.17f, 0);

                animator.SetBool("Bear", true);
                animator.SetTrigger("TransformBear");

                ChangeColliderSize(new Vector2(1.2f, 0.43f), new Vector2(-0.05f, -0.42f));
                isTransformed = true;
                animator.SetFloat("XVelo", speedx);
            }
            else
            {
                groundCheck.localPosition += new Vector3(0, 0.17f, 0);

                animator.SetBool("Bear", false);
                ChangeColliderSize(new Vector2(0.7f, 0.6f), new Vector2(0f, -0.2f));
                isTransformed = false;
                animator.SetFloat("XVelo", speedx);
            }
        }

        //updatetethers via list
        for (int i = 0; i < activeTethers.Count; i++)
        {
            if (activeTethers[i] != null && tetherTargets[i] != null)
            {
                activeTethers[i].SetPosition(0, druidtransform.position);
                activeTethers[i].SetPosition(1, tetherTargets[i].position);

                float distance = Vector2.Distance(druidtransform.position, tetherTargets[i].position);

                if (distance > maxTetherDistance)
                {
                    Debug.Log("Tether too far, breaking...");

                    // Degrow plant
                    DeGrowPlant(tetherTargets[i]);
                }
            }
        }

        //plantframework & GrowableEnemies

        if (Input.GetMouseButtonDown(0))
        {
            if (isTransformed == false)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("GrowPlants", "GrowEnemy"));
                if (hit.collider != null)
                {
                    IGrowablePlant plant = hit.collider.GetComponent<IGrowablePlant>();
                    if (plant != null)
                    {
                        float distance = Vector2.Distance(druidtransform.position, hit.collider.transform.position);
                        if (distance <= maxTetherDistance - 2)
                        {
                            if (!plant.IsGrown && spirits > 0)
                            {
                                // Grow the plant
                                plant.Grow();
                                Debug.Log("Growing");
                                growplant(hit.collider.transform);
                            }
                            else if (plant.CanDie)
                            {
                                // Degrow the plant
                                DeGrowPlant(hit.collider.transform);
                                animator.SetTrigger("Grow");
                            }
                        }
                    }

                    //Enemies
                    else
                    {
                        IGrowableEnemy enemy = hit.collider.GetComponent<IGrowableEnemy>();
                        if (enemy != null)
                        {
                            float distance = Vector2.Distance(druidtransform.position, hit.collider.transform.position);
                            if (distance <= maxTetherDistance - 2)
                            {
                                if (!enemy.Dead)
                                {
                                    if (!enemy.IsGrown && spirits > 0)
                                    {
                                        // Grow the enemy
                                        enemy.Grow();
                                        Debug.Log("Growing");
                                        growplant(hit.collider.transform);
                                    }
                                    else if (enemy.CanDie)
                                    {
                                        // Degrow the enemy
                                        DeGrowPlant(hit.collider.transform);
                                        animator.SetTrigger("Grow");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //call this function to remove active tether
    public void RemoveTether(Transform plantTransform)
    {
        // Find which index in the list this plant is at
        int index = tetherTargets.IndexOf(plantTransform);

        if (index != -1) // if found
        {
            Destroy(activeTethers[index].gameObject);

            activeTethers.RemoveAt(index);
            tetherTargets.RemoveAt(index);
        }
    }

    //call function to kill plant
    private void DeGrowPlant(Transform planttransform)
    {
        IGrowablePlant plant = planttransform.GetComponent<IGrowablePlant>();
        if (plant != null)
        {
            spirits++;
            plant.Die();
            RemoveTether(planttransform);
        }
        else
        {
            IGrowableEnemy enemy = planttransform.GetComponent<IGrowableEnemy>();
            if (enemy != null)
            {
                spirits += 3;
                enemy.Die();
                RemoveTether(planttransform);
            }
        }
    }

    //removes a spirit and makes the druid do her grow animation and attaches a tether
    private void growplant(Transform plantTransform)
    {
        IGrowablePlant plant = plantTransform.GetComponent<IGrowablePlant>();
        if (plant != null)
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
        else
        {
            IGrowableEnemy enemy = plantTransform.GetComponent<IGrowableEnemy>();
            if (enemy != null)
            {
                LineRenderer tetherclone = Instantiate(tether);
                tetherclone.positionCount = 2;
                tetherclone.SetPosition(0, druidtransform.position);
                tetherclone.SetPosition(1, plantTransform.position);
                tetherclone.useWorldSpace = true;
                activeTethers.Add(tetherclone);
                tetherTargets.Add(plantTransform);

                spirits -= 3;
                animator.SetTrigger("Grow");
            }
        }
    }

    //transformations

    private void ChangeColliderSize(Vector2 newsize, Vector2 newoffset)
    {
        boxcollider.offset = newoffset;
        boxcollider.size = newsize;
    }
}