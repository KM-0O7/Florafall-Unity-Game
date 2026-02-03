using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenAreaReveal : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.75f;
    [SerializeField] private Tilemap hiddenTileMap;
    private bool entered = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!entered)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                entered = true;
                Debug.Log("Entered");
                StartCoroutine(fade(0f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (entered)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                entered = false;
                StartCoroutine(fade(1f));
            }
        }
    }

    private IEnumerator fade(float alpha)
    {
        Color startColor = hiddenTileMap.color;
        float startAlpha = startColor.a;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, alpha, t / fadeTime);
            yield return null;
            hiddenTileMap.color = new Color(startColor.r, startColor.g, startColor.b, a);
        }
        hiddenTileMap.color = new Color(startColor.r,startColor.g,startColor.b, alpha);
    }
}
