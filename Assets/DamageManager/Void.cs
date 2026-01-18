using UnityEngine;

public class Void : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    DruidUI druidUI;
    private GameObject druid;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        druid = GameObject.FindGameObjectWithTag("Player");
        druidUI = druid.GetComponent<DruidUI>();
    }

   
    void Update()
    {
        
    }
}
