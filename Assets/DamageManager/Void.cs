using System.Collections;
using UnityEngine;

public class Void : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    DruidFrameWork druidFrameWork;
    DruidUI druidUI;
    private GameObject druid;
    private Animator circleExpand;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        druid = GameObject.FindGameObjectWithTag("Player");
        druidFrameWork = druid.GetComponent<DruidFrameWork>();
        druidUI = druid.GetComponent<DruidUI>();
        circleExpand = GameObject.FindGameObjectWithTag("CircleExpand").GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            circleExpand.SetTrigger("Expand");
            
            StartCoroutine("Recover");
        }
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.2f);
        druid.transform.position = druidFrameWork.lastGroundPosition;
        yield return new WaitForSeconds(0.3f);
        circleExpand.SetTrigger("Deflate");
        yield return new WaitForSeconds(0.4f);
        druidUI.health -= 1;
    }
}
