using System.Collections;
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

    private float jumpheight = 7;
    private bool bearattackcd = false;
    private bool isTransformed = false;
    private bool isAttacking = false;
    private bool damagecd = false;
    [SerializeField] private GameObject bearattackhitbox;
    private bool istransforming = false;
    [SerializeField] private Animator UIwalker;
    public static bool Transitioning = false;
    private bool transformcd = false;

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
        if (!isAttacking)
        {
            speedx = Input.GetAxisRaw("Horizontal");
            druidrb.linearVelocityX = speedx * druidspeed;
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

            if (!Transitioning)
            {
                UIwalker.SetBool("Backwards", false);
            }
        }
        else if (speedx < 0f)
        {
            druidspriterender.flipX = true;
            if (!Transitioning)
            {
                UIwalker.SetBool("Backwards", true);
            }
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
            if (!isAttacking)
            {
                if (canjump == true)
                {
                    if (druidrb.linearVelocityY > -0.1f)
                    {
                        canjump = false;
                        druidrb.linearVelocityY += jumpheight;
                    }
                }
            }
        }

        //fasterjumpfall
        if (!istransforming)
        {
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
                if (spirits == 5)
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

        //plantframework & GrowableEnemies &  transformation attacks/moves

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
            else if (isTransformed) //transformations
            {
                if (!bearattackcd) //bear attack
                {
                    if (canjump)
                    {
                        StartCoroutine("attack");
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

    private IEnumerator attack()
    {
        bearattackcd = true;
        isAttacking = true;
        canjump = false;

        animator.SetTrigger("BearBite");
        druidrb.linearVelocityX = 0f;

        yield return new WaitForSeconds(0.7f);

        float direction = druidspriterender.flipX ? -1f : 1f;
        RaycastHit2D hit = Physics2D.Raycast(druidtransform.position, new Vector2(direction, 0f), 2.5f, LayerMask.GetMask("GrowEnemy"));
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

    private IEnumerator TransformIntoAnimal(string animal)
    {
        if (animal == "Bear")
        {
            if (transformcd == false)
            {
                canjump = false;
                UIwalker.SetBool("Bear", true);
                animator.SetBool("Bear", true);
                spirits = 0;
                druidrb.linearVelocityX = 0f;
                druidrb.linearVelocityY = 0f;
                druidrb.gravityScale = 0f;

                isAttacking = true;
                animator.SetTrigger("TransformBear");
                istransforming = true;

                yield return new WaitForSeconds(0.4f);//after anim plays
                istransforming = false;
                isAttacking = false;
                druidrb.gravityScale = 1f;
                canjump = true;
                groundCheck.localPosition -= new Vector3(0, 0.17f, 0);
                jumpheight = 6;
                ChangeColliderSize(new Vector2(0.9f, 0.43f), new Vector2(-0.05f, -0.42f));
                isTransformed = true;
                animator.SetFloat("XVelo", speedx);
            }
        }
    }

    private IEnumerator TransformIntoDruid()
    {
        transformcd = true;
        canjump = false;
        UIwalker.SetBool("Bear", false);
        spirits = 5;
        druidrb.linearVelocityX = 0f;
        druidrb.linearVelocityY = 0f;
        druidrb.gravityScale = 0f;

        istransforming = true;
        animator.SetBool("IsTransforming", true);
        isAttacking = true;
        jumpheight = 7;
        animator.SetBool("Bear", false);

        yield return new WaitForSeconds(0.3f);//after anim plays
        animator.SetBool("IsTransforming", false);
        istransforming = false;
        isAttacking = false;
        druidrb.gravityScale = 1f;
        canjump = true;
        groundCheck.localPosition += new Vector3(0, 0.17f, 0);
        ChangeColliderSize(new Vector2(0.7f, 0.6f), new Vector2(0f, -0.2f));
        isTransformed = false;
        animator.SetFloat("XVelo", speedx);

        yield return new WaitForSeconds(1f);
        transformcd = false;
    }
}