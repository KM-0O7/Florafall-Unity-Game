using System.Collections;
using UnityEngine;

public class IcicleFallingPlatform : MonoBehaviour
{
    private Rigidbody2D rig;
    private bool isfalling;
    public float respawntime = 2f;
    public float fallingtime = 1f;
    private Vector2 basespawn;
    private BoxCollider2D collider2Dicicle;
    private Animator animator;
    DruidFrameWork druidFrameWork;
    private GameObject druid;
    [SerializeField] private GameObject icicleHitbox;
    Rigidbody2D druidRig;

    private void Start()
    {
        basespawn = transform.position;
        rig = GetComponent<Rigidbody2D>();
        rig.bodyType = RigidbodyType2D.Kinematic;
        collider2Dicicle = GetComponent<BoxCollider2D>();
        druid = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        druidFrameWork = druid.GetComponent<DruidFrameWork>();
        druidRig = druid.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isfalling)
        {
            if (druidFrameWork.isGrounded)
            {
                playercheck();
            }
        }
    }

    private void playercheck()
    {
        Collider2D col = collider2Dicicle;
        Collider2D playerCol = druid.GetComponent<Collider2D>();
        float contactTolerance = 0.15f;

        float platformTop = col.bounds.max.y;
        float playerFeet = playerCol.bounds.min.y;

        if (playerFeet < platformTop - 0.05f) return;
        if (playerFeet < platformTop - contactTolerance || playerFeet > platformTop + contactTolerance) return;
        if (playerCol.bounds.max.x < col.bounds.min.x || playerCol.bounds.min.x > col.bounds.max.x) return;

        isfalling = true;
        StartCoroutine(platformfall());
    }

    private IEnumerator platformfall()
    {
        var hitbox = icicleHitbox.GetComponent<BoxCollider2D>();
        Debug.Log("Falling");
        isfalling = true;
        yield return new WaitForSeconds(fallingtime);
        hitbox.enabled = true;
        rig.bodyType = RigidbodyType2D.Dynamic;
        rig.gravityScale = 2f;
        collider2Dicicle.enabled = false;
        yield return new WaitForSeconds(respawntime);
        hitbox.enabled = false;

        animator.SetTrigger("respawn");
        rig.bodyType = RigidbodyType2D.Static;
        transform.position = basespawn;
        rig.gravityScale = 0f;

        yield return new WaitForSeconds(0.5f);
        isfalling = false;
        collider2Dicicle.enabled = true;
    }
}