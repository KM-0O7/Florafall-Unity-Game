using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpiritFloat : MonoBehaviour
{
    public float amplitude = 2f;   
    public float speed = 1f;      

    private RectTransform rect;
    private Image img;
    private Vector2 startPos;

    [SerializeField] private Sprite spiritOn;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
     
        startPos = rect.anchoredPosition;
        
        float offset = Random.Range(0f, Mathf.PI * 2f);
        StartCoroutine(FloatRoutine(offset));
    }

    private IEnumerator FloatRoutine(float offset)
    {
        while (true)
        {
            if (img.sprite == spiritOn)
            {
                float t = Time.time * speed + offset;
                rect.anchoredPosition = startPos + new Vector2(0, Mathf.Sin(t) * amplitude);
            }
            else
            {
                
                rect.anchoredPosition = startPos;
                
            }

            yield return null;
        }
    }
}