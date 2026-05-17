using Unity.VisualScripting;
using UnityEngine;

public class Segment : MonoBehaviour
{
    private EnemyDamage damage;
    public int segmentPosition = 0;
    public SegmentHead currentOwner;
    private bool alreadyDead = false;

    private void Start()
    {
        damage = GetComponent<EnemyDamage>();
    }

    private void Update()
    {
        if (damage.dead && !alreadyDead)
        {
            alreadyDead = true;
            currentOwner.Split(segmentPosition);
        }
    }
}
