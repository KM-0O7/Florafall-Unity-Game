using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChunkLoader : MonoBehaviour
{
    /* CHUNKLOADER
     * This script handles moving between scenes via unloading and reloading
     * This script is a singleton
     * This script is persistent
     */
    public static ChunkLoader Instance { get; private set; }

    private string currentChunk;

    /* AWAKE
     * Handles persistence
     */

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

    /* Start
     * Sets the current chunk to the currently loaded scene on start up
     */

    private void Start()
    {
        currentChunk = SceneManager.GetActiveScene().name;
    }

    //Enter chunk starts a courotine which takes a scene name and loads it

    public void EnterChunk(string sceneName, System.Action onChunkLoaded = null)
    {
        StartCoroutine(LoadAndUnload(sceneName, onChunkLoaded));
    }

    /* LOAD AND UNLOAD
     * Checks if it's loading the current scene if it forces reload
     * If its not it loads the scene and while its loading it waits frames until it's done loading
     * Checks if the sceneName is empty and then loads the starting scene via current chunk
     * Sets the current chunk to the now loaded scene if all is good
     */

    private IEnumerator LoadAndUnload(string sceneName, System.Action onChunkLoaded = null)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;
        Debug.Log("Unloaded" + sceneName);

        Scene newScene = SceneManager.GetSceneByName(sceneName);
        while (!newScene.isLoaded)
            yield return null;

        SceneManager.SetActiveScene(newScene);

        yield return null;

        if (AstarPath.active != null)
        {
            AstarPath.active.Scan();
        }

        onChunkLoaded?.Invoke();

        if (!string.IsNullOrEmpty(currentChunk))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentChunk);
            while (!unloadOp.isDone)
                yield return null;
        }

        Debug.Log("Chunkloader Loaded scene " + sceneName + ", unloaded " + currentChunk + ".");

        currentChunk = sceneName;
    }
}