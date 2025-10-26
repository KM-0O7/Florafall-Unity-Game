using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax Factors")]
    [Range(0f, 1f)] public float parallaxFactorX = 0.5f;
    [Range(0f, 1f)] public float parallaxFactorY = 0.5f;

    public float initDelay = 0.1f;

    private Transform cam;
    private Vector3 editorPos;      
    private Vector3 initialCamPos;
    private bool initialized = false;
    private bool initializing = false;

    void Awake()
    {

        editorPos = Application.isPlaying ? transform.position : editorPos;
    }

    void OnEnable()
    {
        cam = Camera.main?.transform;
        initialized = false;
        initializing = false;
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            cam = Camera.main?.transform;
            return;
        }

        if (!initialized && !initializing && Application.isPlaying)
        {
            StartCoroutine(InitializeAfterDelay());
            return;
        }

        if (!initialized) return;

        Vector3 camDelta = cam.position - initialCamPos;

        transform.position = new Vector3(
            editorPos.x + camDelta.x * parallaxFactorX,
            editorPos.y + camDelta.y * parallaxFactorY,
            editorPos.z
        );
    }

    private IEnumerator InitializeAfterDelay()
    {
        initializing = true;
        yield return new WaitForSeconds(initDelay);

        if (cam != null)
        {
        
            initialCamPos = cam.position;
            initialized = true;
        }
    }

#if UNITY_EDITOR
  
    void OnValidate()
    {
        if (!Application.isPlaying)
            editorPos = transform.position;
    }
#endif
}