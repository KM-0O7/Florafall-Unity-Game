using System.Collections;
using UnityEngine;

public class PromptSineWave : MonoBehaviour
{
    public float amplitude = 2f;
    public float speed = 1f;

    private RectTransform rect;

    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        float offset = Random.Range(0f, Mathf.PI * 2);
        StartCoroutine(SineWave(offset));
    }

    private IEnumerator SineWave(float offset)
    {
        while (true)
        {
            float t = Time.time * speed + offset;
            rect.anchoredPosition = startPos + new Vector2(0, Mathf.Sin(t) * amplitude);
            yield return null;
        }
    }
}
