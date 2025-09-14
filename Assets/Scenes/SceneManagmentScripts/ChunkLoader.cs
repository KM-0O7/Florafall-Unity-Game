using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChunkLoader : MonoBehaviour
{
    public static ChunkLoader Instance { get; private set; }

    private HashSet<string> loadedChunks = new HashSet<string>();
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
        Scene firstScene = SceneManager.GetActiveScene();
        currentChunk = firstScene.name;
    }

    public void EnterChunk(string sceneName, System.Action onChunkLoaded = null)
    {
        StartCoroutine(LoadAndUnload(sceneName));
    }

    private IEnumerator LoadAndUnload(string sceneName)
    {
        // Load new chunk
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadOp.isDone) yield return null;
        }

        // Unload previous chunk
        if (!string.IsNullOrEmpty(currentChunk) && currentChunk != sceneName)
        {
            var unloadOp = SceneManager.UnloadSceneAsync(currentChunk);
            while (!unloadOp.isDone) yield return null;
        }

        currentChunk = sceneName;
    }
}