using System.Collections;
using UnityEngine;

public class CliffCutscene : MonoBehaviour
{
    private bool cutsceneCompleted = false;
    private bool inCutscene = false;
    [SerializeField] Transform firstMoveTo;
    DruidGrowFramework DGF;
    Rigidbody2D druidRig;
    Animator druidAnimator;
    Transform druidTransform;
    DruidFrameWork DF;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null )
        {
            DF = player.GetComponent<DruidFrameWork>();
            druidTransform = player.GetComponent<Transform>();
            druidAnimator = player.GetComponent<Animator>();
            DGF = player.GetComponent<DruidGrowFramework>();
            druidRig = player.GetComponent<Rigidbody2D>();
        }
    }
       

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cutsceneCompleted) return;
        if (inCutscene) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            inCutscene = true;
            DruidFrameWork.inCutscene = true;
            CutsceneBars.Instance.CutsceneBarsStart();
            StartCoroutine(CliffCutsceneRoutine());
        }
    }

    private IEnumerator CliffCutsceneRoutine()
    {
        DGF.DeGrowAllPlants();
        druidAnimator.SetFloat("XVelo", 1);
        while (Vector2.Distance(druidTransform.position, firstMoveTo.position) > 0.1)
        {
            druidRig.linearVelocityX = DF.druidspeed;
            yield return null;
        }
        druidRig.linearVelocityX = 0;
        druidAnimator.SetFloat("XVelo", 0);
    }
}
