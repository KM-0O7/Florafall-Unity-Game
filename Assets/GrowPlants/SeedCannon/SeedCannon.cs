using System.Collections;
using UnityEngine;

public class SeedCannon : MonoBehaviour
{
    private Animator animator;
    public bool cannondb = false;
    private bool grew = false;
    private bool shotdb = false;
    [SerializeField] private Transform bulletspawn;
    [SerializeField] private GameObject Bullet;
    private SpriteRenderer cannon;
    public float bulletspeed;
    public bool candie = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cannon = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void GrowGlowRoot()
    {
        if (cannondb == false)
        {
            StartCoroutine(GrowCycle());
        }
    }

    public void die()
    {
        if (cannondb == true)
        {
            if (candie == true)
            {
                StartCoroutine(diecycle());
            }
        }
    }

    private IEnumerator GrowCycle()
    {
        animator.SetTrigger("Grow");
        cannondb = true;
        grew = true;
        yield return new WaitForSeconds(0.75f);
        candie = true;
    }

    private IEnumerator diecycle()
    {
        candie = false;
        animator.SetTrigger("Die");
        grew = false;

        yield return new WaitForSeconds(3f);
        animator.SetTrigger("dbdone");
        cannondb = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (grew && !shotdb)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        shotdb = true;
        yield return new WaitForSeconds(3f);
        Debug.Log("Shooting");
        if (grew && cannondb)
        {
            animator.SetTrigger("Shoot");

            yield return new WaitForSeconds(0.75f);

            //Get Bullets components/add components
            GameObject BulletClone = Instantiate(Bullet, bulletspawn.transform.position, bulletspawn.transform.rotation);

            //Rig
            Rigidbody2D bulletrig = BulletClone.AddComponent<Rigidbody2D>();
            bulletrig.gravityScale = 0;

            //BoxCollider
            BoxCollider2D bulletcollider = BulletClone.AddComponent<BoxCollider2D>();
            bulletcollider.isTrigger = true;

            //Renderer
            SpriteRenderer bulletrender = BulletClone.GetComponent<SpriteRenderer>();
            bulletrender.enabled = true;

            //force

            float direction = cannon.flipX ? -1f : 1f;
            bulletrig.AddForce(Vector2.right * bulletspeed * direction, ForceMode2D.Impulse);
            yield return new WaitForSeconds(2f);
            Destroy(BulletClone);
        }

        shotdb = false;
    }
}