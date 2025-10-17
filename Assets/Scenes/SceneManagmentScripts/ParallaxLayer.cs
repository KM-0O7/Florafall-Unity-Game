using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 2f)]
    public float parallaxFactorX = 0.5f;

    [Range(0f, 2f)]
    public float parallaxFactorY = 0.5f;

    public bool isStatic = false;

    private Transform cam;
    private Vector3 camStartPos;
    private Vector3 layerStartPos;

    private void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;
        layerStartPos = transform.position;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        if (isStatic)
        {
            transform.position = layerStartPos;
            return;
        }

        Vector3 camDelta = cam.position - camStartPos;

        float offsetX = camDelta.x * parallaxFactorX;
        float offsetY = camDelta.y * parallaxFactorY;

        transform.position = new Vector3(layerStartPos.x + offsetX, layerStartPos.y + offsetY, layerStartPos.z);
    }
}