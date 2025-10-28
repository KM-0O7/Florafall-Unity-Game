using UnityEngine;


public class DeadTree : MonoBehaviour, IGrowablePlant
{

    public bool deadtreeDb = false;
    public bool candie = false;
    public bool IsGrown => deadtreeDb;
    public bool CanDie => candie;
    private int spirits = 5;
    public int spiritCost => spirits;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Grow()
    {

    }

    public void Die()
    {

    }
}
