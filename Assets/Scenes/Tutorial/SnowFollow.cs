using UnityEngine;

public class SnowFollow : MonoBehaviour
{
    Transform playerPos;
    Transform snowPosition;
    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        snowPosition = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        snowPosition.position = new Vector3(playerPos.position.x, snowPosition.position.y, snowPosition.position.z);
    }
}
