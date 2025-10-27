using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChunkLoader : MonoBehaviour
{
    public static ChunkLoader Instance { get; private set; }

    private string currentChunk;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentChunk = SceneManager.GetActiveScene().name;
    }

    public void EnterChunk(string sceneName, System.Action onChunkLoaded = null)
    {
        StartCoroutine(LoadAndUnload(sceneName, onChunkLoaded));
    }

    private IEnumerator LoadAndUnload(string sceneName, System.Action onChunkLoaded = null)
    {
        if (currentChunk == sceneName)
        {
            Debug.Log($"[ChunkLoader] Already in scene '{sceneName}', skipping reload.");
            onChunkLoaded?.Invoke();
            yield break;
        }

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);
        onChunkLoaded?.Invoke();

        if (!string.IsNullOrEmpty(currentChunk))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentChunk);
            while (!unloadOp.isDone)
                yield return null;
        }

        currentChunk = sceneName;
        Debug.Log($"[ChunkLoader] Loaded scene '{sceneName}', unloaded '{currentChunk}'.");
    }
}