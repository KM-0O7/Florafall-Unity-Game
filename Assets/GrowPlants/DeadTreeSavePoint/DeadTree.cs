using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadTree : MonoBehaviour, IGrowablePlant
{
    public bool deadtreeDb = false;
    public bool candie = false;
    public bool IsGrown => deadtreeDb;
    public bool CanDie => candie;
    private int spirits = 5;
    public int spiritCost => spirits;
    private Animator druidAnimator;
    private Animator TreeAnimator;
    private Rigidbody2D druidRig;
    private DruidUI UI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            UI = player.GetComponent<DruidUI>();
            druidAnimator = player.GetComponent<Animator>();
            druidRig = player.GetComponent<Rigidbody2D>();
        }

        TreeAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Grow()
    {
        if (!deadtreeDb)
        {
            StartCoroutine(GrowCycle());
        }
    }

    public void Die()
    {
        if (deadtreeDb)
        {
            if (candie)
            {
                StartCoroutine(DieCycle());
            }
        }
    }

    private IEnumerator GrowCycle()
    {
        TreeAnimator.SetTrigger("Grow");
 
        DruidFrameWork.canmove = false;
        druidRig.linearVelocity = new Vector2(0f, 0f);
        druidRig.gravityScale = 0f;
        druidAnimator.SetTrigger("Resting");
        deadtreeDb = true;
        yield return new WaitForSeconds(0.75f);
        candie = true;
        UI.spawnSceneName = SceneManager.GetActiveScene().name;
    }

    private IEnumerator DieCycle()
    {
        DruidFrameWork.canmove = true;
        candie = false;
        druidRig.gravityScale = 1f;
        TreeAnimator.SetTrigger("Die");
        druidAnimator.SetBool("StopRest", true);
        yield return new WaitForSeconds(0.6f);
        druidAnimator.SetBool("StopRest", false);
        deadtreeDb = false;
    }
}