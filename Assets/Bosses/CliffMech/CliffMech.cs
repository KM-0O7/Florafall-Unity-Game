using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffMech : MonoBehaviour
{
    [SerializeField] private float health = 100;
    private bool isJumping = false;
    CliffCutscene cliffCutscene;
    Rigidbody2D bossRig;
    private bool jumpCooldown = false;
    private SpriteRenderer bossSprite;
    Transform druidTransform;
    [SerializeField] private float jumpCooldownTime = 4f;
    [SerializeField] private float jumpDetectionDistance = 3f;
    [SerializeField] private float jumpForceX = 3f;
    [SerializeField] private float jumpForceY = 5f;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private GameObject rocket;
    private bool rocketCooldown = false;
    private bool isShooting = false;
    [SerializeField] private float timeBetweenRockets = 0.2f;
    [SerializeField] private float minimumRocketAmount, maximumRocketAmount;
    private List<GameObject> rocketList = new List<GameObject>();
    private List<Transform> rocketHitPositions = new List<Transform>();
    [SerializeField] private float rocketAirTime = 2f;
    [SerializeField] private float rocketCooldownTime = 6f;

    void Start()
    {
        cliffCutscene = GameObject.Find("Cutscene").GetComponent<CliffCutscene>();
        bossRig = GetComponent<Rigidbody2D>();
        druidTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        bossSprite = GetComponent<SpriteRenderer>();
    }

    
    void Update()
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
                    StartCoroutine(Jump(-jumpForceX));
                } else
                {
                    StartCoroutine(Jump(jumpForceX));
                }
               
            }
        }

        if (!isJumping && !isShooting)
        {
            if (druidPos < 0)
            {
                bossSprite.flipX = true;
            }
            else bossSprite.flipX = false;

            if (!rocketCooldown)
            {
                StartCoroutine(RocketShoot());
            }
        }
    }

    private IEnumerator RocketShoot()
    {
        isShooting = true;
        rocketCooldown = true;
        for (int i = 0; i <= Random.Range(minimumRocketAmount, maximumRocketAmount); i++)
        {
            var newRocket = Instantiate(rocket);
            newRocket.SetActive(true);
            Vector2 direction = druidTransform.position - rocket.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            newRocket.transform.rotation = Quaternion.Euler(0, 0, angle);
            rocketList.Add(newRocket);
            rocketHitPositions.Add(druidTransform);
            StartCoroutine(RocketMove(newRocket, druidTransform));
            yield return new WaitForSeconds(timeBetweenRockets);
        }
        isShooting = false;
        yield return new WaitForSeconds(rocketCooldownTime);
        rocketCooldown = false;
    }

    private IEnumerator RocketMove(GameObject rocket, Transform rocketHitPos)
    {
        float t = 0;
        float amplitude = 2f;
        float frequency = 8f;

        Vector2 startPos = rocket.transform.position;
        Vector2 targetPos = rocketHitPos.position;
        Vector2 direction = (targetPos - startPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        while (t < rocketAirTime)
        {
            Vector2 basePosition = Vector2.Lerp(rocket.transform.position, rocketHitPos.transform.position, t / rocketAirTime);
            float wave = Mathf.Cos(t/rocketAirTime * frequency) * amplitude;
            Vector2 waveOffset = perpendicular * wave;

            rocket.transform.position = basePosition + waveOffset;
            yield return null;
        }
        rocketHitPositions.Remove(rocketHitPos);
        rocketList.Remove(rocket);
    }

    private IEnumerator Jump(float xForce)
    {
        isJumping = true;
        jumpCooldown = true;
        bossRig.AddForce(new Vector2(xForce, jumpForceY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        RaycastHit2D groundRay = Physics2D.Raycast(gameObject.transform.position, Vector2.down, 0.3f, LayerMask.GetMask("Ground"));
        while (!groundRay) yield return null;
        yield return new WaitForSeconds(1f);
        isJumping = false;
        yield return new WaitForSeconds(jumpCooldownTime);
        jumpCooldown = false;
    }
}
