using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CliffMech : MonoBehaviour, IDamageAble
{
    [SerializeField] private float health = 100;
    private bool isJumping = false;
    private CliffCutscene cliffCutscene;
    private Rigidbody2D bossRig;
    private bool jumpCooldown = false;
    private SpriteRenderer bossSprite;
    private Transform druidTransform;
    [SerializeField] private float jumpCooldownTime = 4f;
    [SerializeField] private float jumpDetectionDistance = 3f;
    [SerializeField] private float jumpForceX = 3f;
    [SerializeField] private float jumpForceY = 5f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private GameObject rocket;
    private bool rocketCooldown = false;
    private bool isShooting = false;
    [SerializeField] private float timeBetweenRockets = 0.2f;
    [SerializeField] private float rocketAmount;
    private List<GameObject> rocketList = new List<GameObject>();
    private List<Transform> rocketHitPositions = new List<Transform>();
    [SerializeField] private float rocketCooldownTime = 6f;
    [SerializeField] private Transform checkPointPos;
    [SerializeField] private Transform rocketShootPos;

    //FLASH
    private bool hitImmune = false;

    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float flashPeak = 1f;
    public bool Dead => false;

    private void Start()
    {
        cliffCutscene = GameObject.Find("Cutscene").GetComponent<CliffCutscene>();
        bossRig = GetComponent<Rigidbody2D>();
        druidTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        bossSprite = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (bossRig.bodyType == RigidbodyType2D.Dynamic)
        {
            var druidPos = gameObject.transform.position.x - druidTransform.position.x;
            if (health <= 90)
            {
                cliffCutscene.MechEnd();
            }

            if (!isJumping && !jumpCooldown && !isShooting)
            {
                if (Vector2.Distance(gameObject.transform.position, druidTransform.position) <= jumpDetectionDistance)
                {
                    if (druidPos < 0)
                    {
                        StartCoroutine(Jump(jumpForceX));
                    }
                    else
                    {
                        StartCoroutine(Jump(-jumpForceX));
                    }
                }
            }

            if (!isJumping && !isShooting)
            {
                if (druidPos < 0)
                {
                    bossSprite.flipX = false;
                    bossRig.linearVelocityX = movementSpeed;
                }
                else
                {
                    bossSprite.flipX = true;
                    bossRig.linearVelocityX = -movementSpeed;
                }

                if (!rocketCooldown)
                {
                    StartCoroutine(RocketShoot());
                }
            }

            if (isJumping)
            {
                if (bossRig.linearVelocityX > -0.1)
                {
                    bossRig.gravityScale = 3f;
                }
                else bossRig.gravityScale = 1.5f;
            }
        }
    }

    public void TakeDamage(float damage) //call to take damage put damage in parameters
    {
        if (!hitImmune)
        {
            hitImmune = true;
            health -= damage;
            StartCoroutine(HitImmuneCoroutine(0.5f));
            Flash();
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

    private IEnumerator FlashCoroutine()
    {
        Debug.Log("IceCrowFlash");
        float timer = 0f;

        bossSprite.GetPropertyBlock(mpb);

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float t = 1f - (timer / flashDuration);
            float intensity = t * flashPeak;

            mpb.SetFloat("_FlashIntensity", intensity);
            bossSprite.SetPropertyBlock(mpb);

            yield return null;
        }

        mpb.SetFloat("_FlashIntensity", 0f);
        bossSprite.SetPropertyBlock(mpb);

        flashRoutine = null;
    }

    private IEnumerator HitImmuneCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        hitImmune = false;
    }

    private IEnumerator RocketShoot()
    {
        isShooting = true;
        rocketCooldown = true;
        for (int i = 0; i < rocketAmount; i++)
        {
            Debug.Log("Shooting");
            var newRocket = Instantiate(rocket);
            newRocket.SetActive(true);
            newRocket.transform.position = rocketShootPos.position;
            Vector2 direction = druidTransform.position - newRocket.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            newRocket.transform.rotation = Quaternion.Euler(0, 0, angle);
            rocketList.Add(newRocket);
            rocketHitPositions.Add(druidTransform);
            StartCoroutine(RocketMove(newRocket, druidTransform));
            yield return new WaitForSeconds(timeBetweenRockets);
        }
        yield return new WaitForSeconds(1f);
        isShooting = false;
        yield return new WaitForSeconds(rocketCooldownTime);
        rocketCooldown = false;
    }

    private IEnumerator RocketMove(GameObject rocket, Transform rocketHitPos)
    {
        float amplitude = 0.25f;
        float frequency = 3f;
        float speed = 10f;

        Vector2 startPos = rocket.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(rocketHitPos.position, Vector2.down, 50f, LayerMask.GetMask("Ground"));

        Vector2 targetPos = rocketHitPos.position;
        Vector2 direction = (targetPos - startPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float elapsed = 0f;

        float distance = Vector2.Distance(startPos, targetPos);
        float travelTime = distance / speed;

        while (elapsed < distance)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / travelTime;

            Vector2 basePosition = Vector2.Lerp(startPos, targetPos, progress);

            float wave = Mathf.Cos(elapsed * frequency * Mathf.PI * 2f) * amplitude;
            Vector2 waveOffset = perpendicular * wave;

            rocket.transform.position = basePosition + waveOffset;
            yield return null;
        }
        rocketHitPositions.Remove(rocketHitPos);
        rocketList.Remove(rocket);
    }

    private IEnumerator Jump(float xForce)
    {
        Debug.Log("Jumping");
        isJumping = true;
        jumpCooldown = true;
        bossRig.AddForce(new Vector2(xForce, jumpForceY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        bool touchingGround = false;
        while (!touchingGround)
        {
            touchingGround = Physics2D.OverlapCircle(checkPointPos.position, 0.2f, LayerMask.GetMask("Ground"));
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        isJumping = false;
        bossRig.gravityScale = 1.5f;
        yield return new WaitForSeconds(jumpCooldownTime);
        jumpCooldown = false;
    }
}