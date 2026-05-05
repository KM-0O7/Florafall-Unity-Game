using UnityEngine;
using System.Collections;

public class SineWave : MonoBehaviour
{
    public float amplitude = 2f;
    public float speed = 1f;

    private Transform rect;

    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<Transform>();
        float offset = Random.Range(0f, Mathf.PI * 2);
        startPos = transform.position;
        StartCoroutine(SineWaveRoutine(offset));
    }

    private IEnumerator SineWaveRoutine(float offset)
    {
        while (true)
        {
            float t = Time.time * speed + offset;
            rect.position = startPos + new Vector2(0, Mathf.Sin(t) * amplitude);
            yield return null;
        }
    }
}
