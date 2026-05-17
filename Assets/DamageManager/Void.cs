using System.Collections;

using UnityEngine;

public class Void : MonoBehaviour
{
    private DruidFrameWork druidFrameWork;
    private DruidUI druidUI;
    private GameObject druid;
    [SerializeField] private bool knockBack = false;
    private Animator circleExpand;
    private Rigidbody2D druidRig;
    private bool alreadyHit = false;
    Vector2 posTP = Vector2.zero;

    private void Start()
    {
        druid = GameObject.FindGameObjectWithTag("Player");
        if (druid)
        {
            druidFrameWork = druid.GetComponent<DruidFrameWork>();
            druidUI = druid.GetComponent<DruidUI>();
            druidRig = druid.GetComponent<Rigidbody2D>();
        }
        circleExpand = TransitionManager.Instance.transitions;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !alreadyHit)
        {
            posTP = druidFrameWork.lastGroundPosition;
            alreadyHit = true;
            DruidFrameWork.canmove = false;
            circleExpand.SetTrigger("Start");
            StartCoroutine("Recover");
            Persistence.instance.ApplyDamageToDruid(druid, 1);
            if (knockBack)
            {
                druidRig.linearVelocity = Vector2.zero;
                var hitYDir = transform.position.y > druid.transform.position.y ? -1 : 1;
                var hitXDir = transform.position.x > druid.transform.position.x ? -1 : 1;
                druidRig.AddForce(new Vector2(hitXDir * 5, hitYDir * 5), ForceMode2D.Impulse);   
            }
        }
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.5f);
        druid.transform.position = posTP;
        circleExpand.SetTrigger("End");
        yield return new WaitForSeconds(0.5f);
        DruidFrameWork.canmove = true;
        alreadyHit = false;
    }
}