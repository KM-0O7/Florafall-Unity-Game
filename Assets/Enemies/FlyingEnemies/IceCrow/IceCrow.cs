using Pathfinding;
using System.Collections;
using UnityEngine;

public class IceCrow : MonoBehaviour, IGrowableEnemy, IDamageAble, IEnemy
{
    /* ICE CROW
     * Handles the Ice Crow AI
     * Includes PathFinding
     * Grow Behaviour
     * Dash Behaviour
     * Shooting Behaviour
     */

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float playerDetectionDistance = 2f;
    [SerializeField] private float dashDetectionDistance = 1.5f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float hoverDistance = 4f;
    [SerializeField] private float bulletXOffset = 2;
    [SerializeField] private float bulletForce = 3f;
    [SerializeField] private float shootingDistance = 6f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float health = 7f;
    [SerializeField] private GameObject spikeObject;
    [SerializeField] private float deActivationDistance = 12f;

    private bool isDashing = false;
    private bool dashCD = false;
    private bool dead;
    public bool Dead => dead;
    private bool growDb;
    public bool IsGrown => growDb;
    private bool candie = false;
    public bool FlyingEnemy => true;
    private bool isLerping = false;
    public bool IsLerping => isLerping;
    public void SetLerp(bool value)
    {
        isLerping = value;
    }
    public bool GroundEnemy => false;
    public int spiritCost => 4;
    public bool CanDie => candie;
    private Rigidbody2D enemyRig;
    private BoxCollider2D spikeGrowHitbox;
    private SpriteRenderer enemySprite;
    private bool isShooting = false;
    private bool canShoot = true;

    private Animator animator;
    private bool playerInSight = false;
    private Transform enemyTransform;
    private Transform playerTransform;
    private int direction;
    private ParticleSystem explodeParticle;

    //FLASH
    private bool hitImmune = false;
    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float flashPeak = 1f;

    //PATHFINDING
    private Seeker seeker;

    private Path path;
    [SerializeField] private float pathRepeatRate = 0.2f;
    public float nextWaypointDistance = 0.2f;
    private int currentWaypoint = 0;

    private void Start()
    {
        explodeParticle = GetComponent<ParticleSystem>();
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        enemyTransform = GetComponent<Transform>();
        enemySprite = GetComponent<SpriteRenderer>();
        enemyRig = GetComponent<Rigidbody2D>();
        spikeGrowHitbox = spikeObject.GetComponent<BoxCollider2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        mpb = new MaterialPropertyBlock();
        enemySprite.material = new Material(enemySprite.material);
        if (player != null)
        {
            playerTransform = player.gameObject.GetComponent<Transform>();
        }

        InvokeRepeating("UpdatePath", 0f, pathRepeatRate);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            var hoverDirection = enemySprite.flipX ? -1f : 1f;
            Vector2 hoverTarget = new Vector2(hoverDirection * hoverDistance + playerTransform.position.x, playerTransform.position.y);
            seeker.StartPath(transform.position, hoverTarget, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!growDb && !isLerping && !dead)
        {
            if (path == null) return;
            if (currentWaypoint >= path.vectorPath.Count) return;

            if (playerInSight && !isShooting && !isDashing)
            {
                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, moveSpeed * Time.fixedDeltaTime);

                float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distance < nextWaypointDistance) currentWaypoint++;
            }
        }
    }

    private void Update()
    {

        //---- DEATH ----
        if (health < 1 || health == 0)
        {
            
            enemyRig.linearVelocityX = 0f;
            enemyRig.linearVelocityY = 0f;
            if (dead == false)
            {
                StartCoroutine(Death());
            }
        }

        if (!growDb && !dead && !isLerping)
        {
            if (!isDashing && !isShooting)
            {
                if (playerTransform.position.x < enemyTransform.position.x)
                {
                    enemySprite.flipX = false;
                }
                else
                {
                    enemySprite.flipX = true;
                }
            }

            direction = enemySprite.flipX ? 1 : -1;
            float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
            if (distance < playerDetectionDistance && playerInSight == false)
            {
                StartCoroutine(PlayerSpotted());
            }

            if (playerInSight)
            {
                // ---- DASH AVOIDANCE ----
                if (isDashing)
                {
                    RaycastHit2D dashRay = Physics2D.Raycast((Vector2)enemyTransform.position, new Vector2(direction, 0), 1, LayerMask.GetMask("Ground", "Breakables"));
                    if (dashRay)
                    {
                        StartCoroutine(StopDash());
                    }
                }

                RaycastHit2D dashHit = Physics2D.Raycast(enemyTransform.position, new Vector2(direction, 0), dashDetectionDistance, LayerMask.GetMask("Player", "Ground"));
                if (dashHit && isDashing == false && dashCD == false && isShooting == false && dashHit.collider.CompareTag("Player"))
                {
                    dashCD = true;
                    isDashing = true;
                    StartCoroutine(Dash());
                }

                if (!isShooting && !isDashing && canShoot)
                {
                    if (Vector2.Distance(enemyTransform.position, playerTransform.position) < shootingDistance)
                    {
                        if (enemyTransform.position.y < (playerTransform.position.y + 0.5f) && enemyTransform.position.y > (playerTransform.position.y - 0.5f))
                        {
                            canShoot = false;
                            isShooting = true;
                            StartCoroutine(shootingCoroutine(Random.Range(4, 7)));
                        }
                    }
                }

                if (Vector2.Distance(gameObject.transform.position, playerTransform.position) > deActivationDistance)
                {
                    playerInSight = false;
                }
            }
        }
    }
    private IEnumerator Death()
    {
        enemySprite.enabled = false;
        explodeParticle.Emit(10);
        dead = true;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

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

    public void Grow()
    {
        if (!dead)
        {
            if (IsGrown == false)
            {
                if (candie == false)
                {
                    growDb = true;
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
            if (growDb == true)
            {
                if (candie == true)
                {
                    growDb = false;
                    candie = false;
                    StartCoroutine(DieCycle());
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, new Vector3(direction * dashDetectionDistance, 0, 0), Color.red);
        Gizmos.DrawWireSphere(transform.position, playerDetectionDistance);
        Gizmos.DrawWireSphere(transform.position, shootingDistance);
    }

    private IEnumerator FlashCoroutine()
    {
        Debug.Log("IceCrowFlash");
        float timer = 0f;

        enemySprite.GetPropertyBlock(mpb);

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float t = 1f - (timer / flashDuration);
            float intensity = t * flashPeak;

            mpb.SetFloat("_FlashIntensity", intensity);
            enemySprite.SetPropertyBlock(mpb);

            yield return null;
        }

        mpb.SetFloat("_FlashIntensity", 0f);
        enemySprite.SetPropertyBlock(mpb);

        flashRoutine = null; 
    }

    private IEnumerator HitImmuneCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        hitImmune = false;
    }

    private IEnumerator GrowCycle()
    {
        enemyRig.gravityScale = 1f;
        spikeGrowHitbox.isTrigger = true;
        animator.SetTrigger("Grow");
        yield return null;
    }

    private IEnumerator DieCycle()
    {
        enemyRig.gravityScale = 0f;
        spikeGrowHitbox.isTrigger = false;
        animator.SetTrigger("UnGrow");
        yield return null;
    }


    private IEnumerator StopDash()
    {
        isDashing = false;
        animator.SetTrigger("StopDash");
        enemyRig.linearVelocityX = 0;
        yield return new WaitForSeconds(Random.Range(5, 7));
        dashCD = false;
    }

    private IEnumerator shootingCoroutine(float shotCd)
    {
        animator.SetTrigger("Shoot");
        enemyRig.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(0.4f);
        Debug.Log("EnemyShooting");
        isShooting = false;

        GameObject bulletClone = Instantiate(bullet);
        bulletClone.transform.position = (Vector2)transform.position + new Vector2(bulletXOffset * direction, 0);
        Rigidbody2D bulletRig = bulletClone.GetComponent<Rigidbody2D>();
        SpriteRenderer bulletSprite = bulletClone.GetComponent<SpriteRenderer>();
        bulletSprite.enabled = true;
        bulletRig.AddForce(Vector2.right * direction * bulletForce, ForceMode2D.Impulse);
        bulletClone.AddComponent<BoxCollider2D>();
        BoxCollider2D box = bulletClone.GetComponent <BoxCollider2D>();
        box.size = new Vector2(0.4f, 0.1f);
        box.isTrigger = true;
        box.enabled = true;
    
        bulletSprite.flipX = enemySprite.flipX;

        yield return new WaitForSeconds(1f);
        if (bulletClone)
        {
            Animator bulletAnimator = bulletClone.GetComponent<Animator>();
            if (bulletAnimator != null)
            {
                bulletAnimator.SetTrigger("Decay");
            }
            yield return new WaitForSeconds(0.3f);
            Destroy(bulletClone);
        }

        yield return new WaitForSeconds(shotCd);
        canShoot = true;
    }

    private IEnumerator PlayerSpotted()
    {
        enemyRig.linearVelocityX = 0;
        animator.SetTrigger("SpotPlayer");
        yield return new WaitForSeconds(0.35f);
        playerInSight = true;
    }

    private IEnumerator Dash()
    {
        var direction = enemySprite.flipX ? 1 : -1;
        enemyRig.linearVelocityX = 0;
        animator.SetTrigger("Dash");
        yield return new WaitForSeconds(0.5f);
        enemyRig.linearVelocityX += dashForce * direction;
        yield return new WaitForSeconds(0.8f);
        if (isDashing)
        {
            StartCoroutine(StopDash());
        }
    }
}