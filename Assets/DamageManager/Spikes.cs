using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    GameObject druid;
    Rigidbody2D druidRig;
    DruidUI druidUI;
    [SerializeField] private float knockBackForce = 2f;
    void Start()
    {
        druid = GameObject.FindGameObjectWithTag("Player"); 
        druidRig = druid.GetComponent<Rigidbody2D>();
        druidUI = druid.GetComponent<DruidUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!druidUI.hitImmune && !druidUI.dead)
            {
                StartCoroutine(KnockBack());
            }
        }
    }

    private IEnumerator KnockBack()
    {
        druidRig.linearVelocity = Vector2.zero;
        DruidFrameWork.canmove = false;
        Persistence.instance.ApplyDamageToDruid(druid, 1f);
        var hitYDir = transform.position.y > druid.transform.position.y ? -1 : 1;
        var hitXDir = transform.position.x > druid.transform.position.x ? -1 : 1;
        druidRig.AddForce(new Vector2(hitXDir * knockBackForce, hitYDir * knockBackForce), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        DruidFrameWork.canmove = true;
    }
}
