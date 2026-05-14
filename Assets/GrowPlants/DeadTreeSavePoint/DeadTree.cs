using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadTree : MonoBehaviour, IGrowablePlant
{
    public bool deadtreeDb = false;
    public bool candie = false;
    public bool IsGrown => deadtreeDb;
    public bool CanDie => candie;
    public bool waterGrown = false;
    public bool WaterGrown => waterGrown;
    private bool canGrow = true;
    public bool CanGrow => canGrow;
    public void setWaterGrow(bool value)
    {
        waterGrown = value;
    }
    private int spirits = 5;
    public int spiritCost => spirits;
    private Animator druidAnimator;
    private Animator TreeAnimator;
    private Rigidbody2D druidRig;
    private DruidUI UI;

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

    
    private void Update()
    {
    }

    public void Grow()
    {
        if (!deadtreeDb && canGrow)
        {
            StartCoroutine(GrowCycle());
        }
    }

    public void Die()
    {
        if (deadtreeDb)
        {
            if (candie && canGrow)
            {
                StartCoroutine(DieCycle());
            }
        }
    }

    private IEnumerator GrowCycle()
    {
        TreeAnimator.SetTrigger("Grow");
        canGrow = false;
        DruidFrameWork.canmove = false;
        druidRig.linearVelocity = new Vector2(0f, 0f);
        druidRig.gravityScale = 0f;
        druidAnimator.SetTrigger("Resting");
        deadtreeDb = true;
        yield return new WaitForSeconds(0.75f);
        canGrow = true;
        candie = true;
        UI.spawnSceneName = SceneManager.GetActiveScene().name;
        UI.currentRespawnPointName = gameObject.name;
    }

    private IEnumerator DieCycle()
    {
        DruidFrameWork.canmove = true;
        canGrow = false;
        candie = false;
        druidRig.gravityScale = 1f;
        TreeAnimator.SetTrigger("Die");
        druidAnimator.SetBool("StopRest", true);
        yield return new WaitForSeconds(0.6f);
        canGrow = true;
        druidAnimator.SetBool("StopRest", false);
        deadtreeDb = false;
    }
}