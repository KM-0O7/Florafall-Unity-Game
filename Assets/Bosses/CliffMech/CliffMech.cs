using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    private Transform rocketShootPos;
    [SerializeField] private Transform normalPos;
    [SerializeField] private Transform flipPos;
    [SerializeField] private float offset = 3f;
    [SerializeField] private float laserCooldownTime = 7f;
    private bool laserCooldown = false;
    [SerializeField] private LineRenderer laser;
    [SerializeField] private Transform laserPos;
    [SerializeField] private float laserSpeed = 2f;
    [SerializeField] private float laserWalkSpeed = 1f;
    [SerializeField] private bool laserOn = false;
    [SerializeField] private float laserCutOffDistance = 2f;
    [SerializeField] private bool mechDone = false;
    DruidUI druidUI;
    [SerializeField] private float timeConstraint = 180f;
    [SerializeField] private GameObject laserCannon;
    [SerializeField] private Animator laserCannonAnimator;
    private Animator mechAnimator;

    //FLASH
    private bool hitImmune = false;

    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float flashPeak = 1f;
    public bool Dead => false;
    

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        cliffCutscene = GameObject.Find("Cutscene").GetComponent<CliffCutscene>();
        mechAnimator = GetComponent<Animator>();
        bossRig = GetComponent<Rigidbody2D>();
        druidUI = player.GetComponent<DruidUI>();
        druidTransform = player.GetComponent<Transform>();
        bossSprite = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    float t = 0;
    private void Update()
    {
        if (bossRig.bodyType == RigidbodyType2D.Dynamic && !mechDone)
        {
            t += Time.deltaTime;
            var druidPos = gameObject.transform.position.x - druidTransform.position.x;
            if (health <= 90 || druidUI.health == 1 || t >= timeConstraint)
            {
                mechDone = true;
                cliffCutscene.MechEnd();
            }

            if (!isJumping && !jumpCooldown && !isShooting && !laserOn)
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

            Vector2 movePos = new Vector2(druidTransform.position.x, gameObject.transform.position.y) + new Vector2((bossSprite.flipX ? -1 : 1) * offset, 0);
            if (!isJumping && !isShooting)
            {
                Debug.Log("Moving");
                gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, movePos, movementSpeed * Time.deltaTime);
            }
            if (druidPos < 0)
            {
                rocketShootPos = flipPos;
                bossSprite.flipX = true;
            }
            else
            {
                bossSprite.flipX = false;
                rocketShootPos = normalPos;

            }

            if (!rocketCooldown && !isJumping && !laserOn)
            {
                StartCoroutine(RocketShoot());
            }

            if (!laserCooldown && !isJumping && !isShooting)
            {
                StartCoroutine(LaserCoroutine());
            }
            

            if (isJumping)
            {
                if (bossRig.linearVelocityY > -0.1)
                {
                    bossRig.gravityScale = 2f;
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

    private IEnumerator LaserCoroutine()
    {
        laserOn = true;
        laserCooldown = true;
        laserCannonAnimator.SetTrigger("Charge");
        yield return new WaitForSeconds(0.4f);
        var newLaser = Instantiate(laser);
        newLaser.positionCount = 2;
        newLaser.SetPosition(0, laserPos.position);
        int direction = bossSprite.flipX ? 1 : -1;
        float laserOffset = 5f;
        var offsetPosition = druidTransform.position.x + (direction * laserOffset);
        RaycastHit2D hit = Physics2D.Raycast(druidTransform.position, Vector2.down, 50f, LayerMask.GetMask("Ground"));
        Vector2 hitPos = new Vector2(offsetPosition, hit.point.y); 
 
        newLaser.SetPosition(1, hitPos);
        movementSpeed -= laserWalkSpeed;
     
        while (Vector2.Distance(newLaser.GetPosition(1), gameObject.transform.position) > laserCutOffDistance)
        {
            hitPos.x -= (direction * laserSpeed) * Time.deltaTime;
            Vector3 dir = (Vector3) hitPos - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            laserCannon.transform.rotation = Quaternion.Euler(0, 0, angle - 180);
            newLaser.SetPosition(1, hitPos);
            newLaser.SetPosition(0, laserPos.position);
            yield return null;
        }
        Destroy(newLaser);
        movementSpeed += laserWalkSpeed;
        yield return new WaitForSeconds(0.3f);
        laserCannonAnimator.SetTrigger("Stop");
        yield return new WaitForSeconds(1f);
        laserOn = false;
        yield return new WaitForSeconds(laserCooldownTime);
        laserCooldown = false;
    }

    private IEnumerator RocketShoot()
    {
        isShooting = true;
        rocketCooldown = true;
        mechAnimator.SetTrigger("Shoot");
        for (int i = 0; i < rocketAmount; i++)
        {
            if (mechDone) break;
            Debug.Log("Shooting");
            var newRocket = Instantiate(rocket);
            newRocket.SetActive(true);
            newRocket.transform.position = rocketShootPos.position;
            newRocket.transform.rotation = Quaternion.Euler(0, 0, 90);
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
        mechAnimator.SetTrigger("Fire");
        var rocketLine = rocket.GetComponent<LineRenderer>();
        rocketLine.positionCount = 2;
        float launchTime = 1f;

        Vector2 startPos = rocket.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(rocketHitPos.position, Vector2.down, 50f, LayerMask.GetMask("Ground"));

        float t = 0;
        float launchHeight = 20;
        Vector2 launchPos = new Vector2(rocket.transform.position.x, rocket.transform.position.y + launchHeight);
        while (t < launchTime)
        {
            t += Time.deltaTime;
            rocket.transform.position = Vector2.Lerp(startPos, launchPos, t / launchTime);
            yield return null;
        }

        Vector2 targetPos = rocketHitPos.position;

        rocket.transform.rotation = Quaternion.Euler(0, 0, -90f);
        float speed = 5f;

        while (Mathf.Abs(druidTransform.position.x - rocket.transform.position.x) > 0.5f)
        {
            RaycastHit2D beamCast = Physics2D.Raycast(rocket.transform.position, Vector2.down, 100, LayerMask.GetMask("Ground"));
            rocketLine.SetPosition(0, rocket.transform.position);
            rocketLine.SetPosition(1, beamCast.point);
            Vector2 rotationPos = new Vector2(druidTransform.position.x, rocket.transform.position.y);
            rocket.transform.position = Vector2.MoveTowards(rocket.transform.position, rotationPos, speed * Time.deltaTime);

            yield return null;
            targetPos = beamCast.point;

        }

        int flashCount = 5;
        for (int i = 0; i < flashCount; i++)
        {
            rocketLine.startColor = Color.white;
            rocketLine.endColor = Color.white;
            yield return new WaitForSeconds(0.1f);
            rocketLine.startColor = Color.red;
            rocketLine.endColor = Color.red;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(rocketLine);
        t = 0;
        float diveTime = 0.7f;
        startPos = rocket.transform.position;

        while (t < diveTime)
        {
            rocket.transform.position = Vector2.Lerp(startPos, targetPos, t / diveTime);
            t += Time.deltaTime;
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