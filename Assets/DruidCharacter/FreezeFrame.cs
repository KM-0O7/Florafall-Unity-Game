using System.Collections;
using UnityEngine;

public class FreezeFrame : MonoBehaviour 
{
    public static FreezeFrame Instance;

    private void Awake()
    {
        // Make this a singleton for easy access
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TriggerFreeze(float duration, float timeScale = 0f)
    {
       StartCoroutine(Freeze(duration, timeScale));
    }

    private IEnumerator Freeze(float duration, float timeScale)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration); // Use real time!
        Time.timeScale = originalTimeScale;
    }
}
