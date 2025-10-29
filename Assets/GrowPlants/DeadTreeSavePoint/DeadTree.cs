using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            druidAnimator = player.GetComponent<Animator>();
        }

        TreeAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Grow()
    {
    }

    public void Die()
    {
    }
}