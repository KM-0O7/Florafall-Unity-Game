using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public static Transform target;
    public GameObject Maincharacter;
    private float followspeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        target = Maincharacter.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        target = Maincharacter.transform;
        Vector3 newpos = new Vector3(target.position.x, target.position.y, -10);
        transform.position = Vector3.Slerp(transform.position, newpos, followspeed * Time.deltaTime);
    }
}