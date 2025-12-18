using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class IceCrow : MonoBehaviour, IGrowableEnemy
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float playerDetectionDistance = 2f;
    [SerializeField] private float wanderRadius = 1.5f;
    [SerializeField] private float dashDetectionDistance = 1.5f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float hoverDistance = 4f;
    [SerializeField] private float dashAttackDetectionDistance = 3f;
    [SerializeField] private float bulletXOffset = 2;
    [SerializeField] private float wallDetectionDistance = 1f;
    [SerializeField] private Vector2 wallDetectionBoxSize;

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
    private GameObject bullet;
    private Animator animator;
    private bool playerInSight = false;
    private Transform enemyTransform;
    private Transform playerTransform;
    private int direction;
    

    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyTransform = GetComponent<Transform>();
        enemySprite = GetComponent<SpriteRenderer>();
        enemyRig = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {        
            playerTransform = player.gameObject.GetComponent<Transform>();
        }
    }

    private void Update()
    {
        direction = enemySprite.flipX ? 1 : -1;
        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        if (distance < playerDetectionDistance)
        {
            playerInSight = true;
            hasReachedTarget = true;
        }
        else
        {
          
            playerInSight = false;
        }

        Collider2D wall = Physics2D.OverlapBox(transform.position, wallDetectionBoxSize, 0f, LayerMask.GetMask("Ground"));
        if (wall != null)
        {

        }

        if (playerInSight)
        {

            //Move towards player
            float target = playerTransform.position.x + (enemySprite.flipX ? -hoverDistance : hoverDistance);


            if (!isDashing)
            {
                enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, new Vector2(target, playerTransform.position.y), moveSpeed * Time.deltaTime);
            }
          
            
            RaycastHit2D dashHit = Physics2D.Raycast(enemyTransform.position, new Vector2(direction, 0), dashDetectionDistance, LayerMask.GetMask("Player"));

            if (dashHit && isDashing == false && dashCD == false)
            {
                dashCD = true;
                isDashing = true;
                StartCoroutine(Dash());
            }

            if (isDashing == false)
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
         
          

            if (!isShooting)
            {
                isShooting = true;
                StartCoroutine(shootingCoroutine(Random.Range(2, 4)));
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

            if (randomTarget.x > enemyTransform.position.x)
            {
                enemySprite.flipX = true;
            } else
            {
                enemySprite.flipX = false;
            }
            
        }          
    }  

    public void Grow()
    {

    }

    public void Die()
    {

    }

    private IEnumerator shootingCoroutine(float shotCd) 
    {
        GameObject bulletClone = Instantiate(bullet);
        bulletClone.transform.position = (Vector2) enemyTransform.position * direction + new Vector2(bulletXOffset, 0);
        
        yield return new WaitForSeconds(shotCd);
        isShooting = false; 
    }

    private IEnumerator Dash()
    {
        var direction = enemySprite.flipX ? 1 : -1;
        enemyRig.linearVelocityX += dashForce * direction;
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        enemyRig.linearVelocityX = 0;

        yield return new WaitForSeconds(10f);
        dashCD = false;

    }
}