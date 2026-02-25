using UnityEngine;

public class CliffMech : MonoBehaviour
{
    [SerializeField] private float health = 100;
    private bool isJumping = false;
    CliffCutscene cliffCutscene;
    Rigidbody2D bossRig;
    private bool jumpCooldown = false;
    Transform druidTransform;
    [SerializeField] private float jumpDetectionDistance = 3f;
    [SerializeField] private float jumpForce = 3f;
    void Start()
    {
        cliffCutscene = GameObject.Find("Cutscene").GetComponent<CliffCutscene>();
        bossRig = GetComponent<Rigidbody2D>();
        druidTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    
    void Update()
    {
        if (health <= 90)
        {
            cliffCutscene.MechEnd();
        }

        if (!isJumping && !jumpCooldown)
        {
            if (Vector2.Distance(gameObject.transform.position, druidTransform.position) <= jumpDetectionDistance)
            {
                var druidPos = gameObject.transform.position.x - druidTransform.position.x;
                if (druidPos < 0)
                {
                    Jump(-jumpForce);
                } else
                {
                    Jump(jumpForce);
                }
               
            }
        }
    }

    private void Jump(float xForce)
    {
        
    }
}
