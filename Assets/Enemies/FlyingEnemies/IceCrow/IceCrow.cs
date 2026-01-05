using Pathfinding;
using System.Collections;
using UnityEngine;

public class IceCrow : MonoBehaviour, IGrowableEnemy
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float playerDetectionDistance = 2f;
    [SerializeField] private float wanderRadius = 1.5f;
    [SerializeField] private float dashDetectionDistance = 1.5f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float hoverDistance = 4f;
    [SerializeField] private float bulletXOffset = 2;
    [SerializeField] private float wallDetectionDistance = 1f;
    [SerializeField] private float bulletForce = 3f;
    [SerializeField] private float shootingDistance = 6f;
    [SerializeField] private GameObject bullet;

    private bool isDashing = false;
    private bool dashCD = false;
    private bool dead;
    public bool Dead => dead;
    private bool growDb;
    public bool IsGrown => growDb;
    private bool candie = false;
    public bool CanDie => candie;
    private Rigidbody2D enemyRig;
    private SpriteRenderer enemySprite;
    private bool isShooting = false;
    private bool hasReachedTarget = true;
    private Vector2 randomTarget;
    private bool canShoot = true;
  
    private Animator animator;
    private bool playerInSight = false;
    private Transform enemyTransform;
    private Transform playerTransform;
    private int direction;

    //PATHFINDING
    private Seeker seeker;
    private Path path;
    [SerializeField] private float pathRepeatRate = 0.2f;
    public float nextWaypointDistance = 0.2f;
    private int currentWaypoint = 0;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        enemyTransform = GetComponent<Transform>();
        enemySprite = GetComponent<SpriteRenderer>();
        enemyRig = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
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
            seeker.StartPath(transform.position, playerTransform.position, OnPathComplete);
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

    private void Update()
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

        //---- WANDER ----
        if (!playerInSight)
        {
            if (hasReachedTarget)
            {
                hasReachedTarget = false;
                randomTarget = (Vector2)enemyTransform.position + Random.insideUnitCircle * wanderRadius;
            }

            enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, randomTarget, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(enemyTransform.position, randomTarget) < 0.1 && hasReachedTarget == false)
            {
                hasReachedTarget = true;
            }

            if (!hasReachedTarget || playerInSight)
            {
                if (randomTarget.x > enemyTransform.position.x)
                {
                    enemySprite.flipX = true;
                }
                else
                {
                    enemySprite.flipX = false;
                }
            }

        }

        direction = enemySprite.flipX ? 1 : -1;
        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        if (distance < playerDetectionDistance && playerInSight == false && hasReachedTarget == false)
        {
            StartCoroutine(PlayerSpotted());
        }

        if (playerInSight)
        {
            
            // ---- DASH AVOIDANCE ----
            if (isDashing)
            {
                RaycastHit2D dashRay = Physics2D.Raycast((Vector2)enemyTransform.position, new Vector2(direction, 0), 1, LayerMask.GetMask("Ground"));
                if (dashRay)
                {
                    StartCoroutine(StopDash());
                }
            }

            RaycastHit2D dashHit = Physics2D.Raycast(enemyTransform.position, new Vector2(direction, 0), dashDetectionDistance, LayerMask.GetMask("Player"));

            if (dashHit && isDashing == false && dashCD == false && isShooting == false)
            {
                dashCD = true;
                isDashing = true;
                StartCoroutine(Dash());
            }

          

            if (!isShooting && !isDashing && canShoot && !dashCD)
            {
                if (Vector2.Distance(enemyTransform.position, playerTransform.position) < shootingDistance)
                {
                    if (enemyTransform.position.x < (playerTransform.position.x + 1) && enemyTransform.position.x < (playerTransform.position.x - 1))
                    {
                        canShoot = false;
                        isShooting = true;
                        StartCoroutine(shootingCoroutine(Random.Range(2, 4)));
                    }
                }
            }
        }
    }

    public void Grow()
    {
    }

    public void Die()
    {
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, new Vector3(direction * dashDetectionDistance, 0, 0), Color.red);
        Gizmos.DrawWireSphere(transform.position, playerDetectionDistance);
        Gizmos.DrawWireSphere(transform.position, shootingDistance);

    }

    private IEnumerator StopDash()
    {
        isDashing = false;
        animator.SetTrigger("StopDash");
        enemyRig.linearVelocityX = 0;
        yield return new WaitForSeconds(5f);
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

        bulletSprite.flipX = enemySprite.flipX;

        yield return new WaitForSeconds(2f);
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
        hasReachedTarget = true;
        animator.SetTrigger("SpotPlayer");
        yield return new WaitForSeconds(0.3f);
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