using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private GameObject boss;
    Transform bossTranform;
    Rigidbody2D bossRig;
    BoxCollider2D bossCollider;
    [SerializeField] private float zoomedOutSize = 1.5f;
    private PixelPerfectCamera ppc;
    private int startPPU;
    [SerializeField] private int endPPU = 20;
    [SerializeField] private Transform firstCamLerpPos;

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
        if (boss != null)
        {
            bossTranform = boss.GetComponent<Transform>();
            bossRig = boss.GetComponent<Rigidbody2D>();
            bossCollider = boss.GetComponent<BoxCollider2D>();
        }
        ppc = Camera.main.GetComponent<PixelPerfectCamera>();
        startPPU = ppc.assetsPPU;
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
        druidAnimator.SetTrigger("Reset");
        DGF.DeGrowAllPlants();
        druidAnimator.SetFloat("XVelo", 1);
        druidAnimator.SetFloat("YVelo", 0);
        while (Vector2.Distance(druidTransform.position, firstMoveTo.position) > 0.1)
        {
            druidRig.linearVelocityX = DF.druidspeed;
            yield return null;
        }
        druidRig.linearVelocityX = 0;
        druidAnimator.SetFloat("XVelo", 0);
        ppc.assetsPPU = ppc.assetsPPU;
        ppc.enabled = false;
        float startOrtho = Camera.main.orthographicSize;
        float t = 0;
        FollowPlayer followPlayer = Camera.main.GetComponent<FollowPlayer>();

        var currentCamPos = Camera.main.transform.position;
        followPlayer.enabled = false;
        while (t < 0.75f)
        {
            t += Time.deltaTime;
            float k = t / 0.75f;
            k = 1f - Mathf.Cos(k * Mathf.PI * 0.5f);
            Camera.main.orthographicSize = Mathf.Lerp(startOrtho, zoomedOutSize, k);
            float newX = Mathf.Lerp(currentCamPos.x, firstCamLerpPos.position.x, k);
            float newY = Mathf.Lerp(currentCamPos.y, firstCamLerpPos.position.y, k);

            Camera.main.transform.position = new Vector3(newX, newY, currentCamPos.z);

            yield return null;
        }
        t = 0;
        bossCollider.enabled = true;
        bossRig.bodyType = RigidbodyType2D.Dynamic;
        ppc.assetsPPU = endPPU;
        ppc.enabled = true;
        yield return new WaitForSeconds(2f);
        druidAnimator.SetTrigger("StaffSlam");
    
       
    }
}
