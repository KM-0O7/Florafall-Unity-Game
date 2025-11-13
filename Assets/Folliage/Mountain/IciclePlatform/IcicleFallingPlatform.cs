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

    private void Start()
    {
        basespawn = transform.position;
        rig = GetComponent<Rigidbody2D>();
        rig.bodyType = RigidbodyType2D.Kinematic;
        collider2Dicicle = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isfalling)
        {
            if (DruidFrameWork.canjump == true)
            {
                playercheck();
          
            }
        }
    }

    private void playercheck()
    {
        Collider2D col = GetComponent<Collider2D>();

        Vector2 checkPoint = new Vector2(transform.position.x, transform.position.y + col.bounds.extents.y + 0.1f);

        Collider2D[] hits = Physics2D.OverlapPointAll(checkPoint);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                StartCoroutine(platformfall());
                break;
            }
        }
    }

    private IEnumerator platformfall()
    {
        isfalling = true;
        yield return new WaitForSeconds(fallingtime);
        rig.bodyType = RigidbodyType2D.Dynamic;
        rig.gravityScale = 2f;
        collider2Dicicle.enabled = false;
        yield return new WaitForSeconds(respawntime);
        animator.SetTrigger("respawn");
        rig.bodyType = RigidbodyType2D.Static;
        transform.position = basespawn;
        rig.gravityScale = 0f;

        yield return new WaitForSeconds(0.5f);
        isfalling = false;
        collider2Dicicle.enabled = true;
    }
}