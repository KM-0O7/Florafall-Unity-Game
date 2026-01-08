using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DruidUI : MonoBehaviour, IDamageAble
{
    public Image[] spiritimages;

    public Sprite fullSpirit;
    public Sprite emptySpirit;
    public int maxSpirits; //change to set max spirits max is 8
    public int spirits; //current spirits
    public Image circleWipe;
    private Transform spawnPoint; //current spawnpoint;
    public float health = 5;
    public float MaxHealth = 5;
    public bool dead = false;
    public GameObject druid;
    [SerializeField] private Animator deathScreen;
    private bool waitCycle = false;
    public string spawnSceneName;
    private Animator druidanims;
    private Rigidbody2D druidRig;
    private bool hitImmune = false;

    private SpriteRenderer spriterenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;
    private DruidFrameWork frameWork;

    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float flashPeak = 1f;

    public bool Dead => dead;



    private void Start()
    {
        spriterenderer = gameObject.GetComponent<SpriteRenderer>();
        spriterenderer.material = new Material(spriterenderer.material);
        frameWork = GetComponent<DruidFrameWork>();
        druidanims = GetComponent<Animator>();
        mpb = new MaterialPropertyBlock();
        health = MaxHealth;
        druidRig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        for (int i = 0; i < spiritimages.Length; i++) //set spirit UI can change in Inspector
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

        if (health <= 0)
        {
            if (dead == false)
            {
                if (!waitCycle)
                {
                    StartCoroutine(DeathScreenCycle());
                }
            }
        }

        if (dead)
        {
            if (waitCycle)
            {
                StartCoroutine(RespawnCycle());
            }
        }
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
                StartCoroutine(frameWork.FreezeFrame(0.3f));
            }
        }
    }

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
        float timer = 0f;

        spriterenderer.GetPropertyBlock(mpb);

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float t = 1f - (timer / flashDuration);
            float intensity = t * flashPeak;

            mpb.SetFloat("_FlashIntensity", intensity);
            spriterenderer.SetPropertyBlock(mpb);

            yield return null;
        }

        mpb.SetFloat("_FlashIntensity", 0f);
        spriterenderer.SetPropertyBlock(mpb);

        flashRoutine = null; // Clear reference
    }

    private IEnumerator DeathScreenCycle()
    {
        health = 0;
        waitCycle = true;
        druidanims.SetTrigger("Death");

        yield return new WaitForSeconds(0.5f);
        deathScreen.SetTrigger("Death");

        yield return new WaitForSeconds(0.3f);
        dead = true;
    }

    private IEnumerator HitImmuneCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        hitImmune = false;
    }

    private IEnumerator RespawnCycle()
    {
        waitCycle = false;
        deathScreen.SetTrigger("Respawn");
        yield return new WaitForSeconds(0.1f);

        Scene currentScene = SceneManager.GetActiveScene();
        druidanims.SetTrigger("Respawn");

        if (SceneManager.GetActiveScene().name != spawnSceneName)
        {
            ChunkLoader.Instance.EnterChunk(spawnSceneName);
        }

        yield return null;
        yield return null;
        yield return null;

        spawnPoint = GameObject.FindWithTag("RespawnPoint")?.transform;
       
        druidRig.gravityScale = 1f;
        health = MaxHealth;
        dead = false;
        spirits = maxSpirits;

        if (spawnPoint != null)
        {
            druid.transform.position = spawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No spawnPoint found in scene!");
        }
    }
}