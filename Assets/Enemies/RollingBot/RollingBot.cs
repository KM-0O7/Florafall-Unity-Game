using System.Collections;
using UnityEngine;

public class RollingBot : MonoBehaviour, IEnemy
{

    //interface
    private bool dead = false;
    public bool Dead => dead;

    [SerializeField] private float health = 4f;

    [SerializeField] private float activationDistance = 15f;
    private Transform druidTransform;
    private Transform rollingBotTransform;
    private SpriteRenderer spriterenderer;
    private MaterialPropertyBlock mpb;
    public float flashDuration = 0.3f;
    public float flashPeak = 1f;
    private Coroutine flashRoutine;
    private Animator animator;
    private Rigidbody2D rb;


    void Start()
    {
        rollingBotTransform = gameObject.GetComponent<Transform>();
        spriterenderer = gameObject.GetComponent<SpriteRenderer>();
        spriterenderer.material = new Material(spriterenderer.material);
        animator = gameObject.GetComponent<Animator>();
        mpb = new MaterialPropertyBlock();
        rb = gameObject.GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            druidTransform = player.GetComponent<Transform>();
        }
    }

    void Update()
    {

        if (health < 1 || health == 0)
        {
            animator.SetTrigger("Death");
            rb.linearVelocityX = 0f;
            rb.linearVelocityY = 0f;
            if (dead == false)
            {
                dead = true;
            }
        }

        // ---- ACTIVATION ----
        if (druidTransform != null && rollingBotTransform.transform != null)
        {
            float distance = Vector2.Distance(druidTransform.position, rollingBotTransform.position);

            if (distance <= activationDistance)
            {

                Debug.Log(gameObject.name + " Is activating!");
                animator.SetTrigger("Activate");
            }
        } else
        {
            Debug.LogError("Druidtransform & rollingBotTransform aren't assigned! in rollingBot Script!");
        }
    }

    public void TakeDamage(float damage) //call to take damage put damage in parameters
    {
        if (!dead)
        {
            health -= damage;
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

    //shows activation range when selected in editor
    void OnDrawGizmosSelected()
    {
     
        Gizmos.color = Color.red;

        
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
