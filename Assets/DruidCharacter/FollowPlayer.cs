using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public static Transform target;
    public GameObject Maincharacter;

    private Vector2 minBounds;// bottom-left of room
    private Vector2 maxBounds; // top-right of room

    private float camHalfWidth;
    private float camHalfHeight;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.25f;
    private bool snapThisFrame = false;
    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        target = Maincharacter.transform;

        cam = GetComponent<Camera>();

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    // Update is called once per frame
    private void Update()
    {
        target = Maincharacter.transform;
        Vector3 newpos = new Vector3(target.position.x, target.position.y, -10);

        float clampedX = Mathf.Clamp(newpos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        float clampedY = Mathf.Clamp(newpos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        Vector3 clampedTarget = new Vector3(clampedX, clampedY, -10);

        if (snapThisFrame)
        {
            transform.position = clampedTarget;

            if (Vector3.Distance(transform.position, clampedTarget) < 0.01f)
            {
                snapThisFrame = false;
            }
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, clampedTarget, ref velocity, smoothTime);
        }
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
    }

    public void SnapToTarget() //call to snap to Target
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -10);
        float clampedX = Mathf.Clamp(newPos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        float clampedY = Mathf.Clamp(newPos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        transform.position = new Vector3(clampedX, clampedY, -10);

        snapThisFrame = true;
        velocity = Vector3.zero;
    }
}