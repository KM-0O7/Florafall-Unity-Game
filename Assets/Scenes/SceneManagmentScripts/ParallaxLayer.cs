using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 2f)]
    public float parallaxFactorX = 0.5f;  // Horizontal parallax

    private Transform cam;
    private Vector3 startPos;

    private void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position; // Remember original tilemap position
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // Calculate offsets based on camera movement
        float offsetX = cam.position.x * parallaxFactorX;
      

        // Apply offsets relative to the original position
        transform.position = new Vector3(startPos.x + offsetX, cam.position.y, startPos.z);
       
    }
}