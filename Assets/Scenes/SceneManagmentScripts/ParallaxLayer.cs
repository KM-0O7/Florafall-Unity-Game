using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 2f)]
    public float parallaxFactor = 0.5f;

    private Transform cam;
    private Vector3 startPos;

    private void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position; // remember original tilemap position
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // Calculate X offset based on camera movement
        float offsetX = (cam.position.x * parallaxFactor);

        // Apply offset relative to original starting position
        transform.position = new Vector3(startPos.x + offsetX, startPos.y, startPos.z);
    }
}