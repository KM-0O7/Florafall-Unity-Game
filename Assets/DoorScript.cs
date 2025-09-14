using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [Header("Target Chunk")]
    public SceneField targetChunk;       // Drag your scene here

    public Animator transitions;
    public string targetSpawnID;       // Name of spawn point in target scene

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(TeleportPlayer(other));
    }

    private IEnumerator TeleportPlayer(Collider2D player)
    {
        transitions.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        DruidFrameWork druid = player.GetComponent<DruidFrameWork>();
        if (druid != null)
        {
            druid.spirits = druid.maxSpirits;
        }
        else
        {
            Debug.LogWarning("no framework lol");
        }
        yield return new WaitForSeconds(0.3f);
        transitions.SetTrigger("End");

        //set spirits to max when changing scene

        // Load the target chunk
        ChunkLoader.Instance.EnterChunk(targetChunk.SceneName);

        // Wait until scene is fully loaded
        Scene targetScene = SceneManager.GetSceneByName(targetChunk.SceneName);
        while (!targetScene.isLoaded)
            yield return null;

        // Find spawn point recursively
        Transform spawnPoint = null;
        foreach (GameObject root in targetScene.GetRootGameObjects())
        {
            spawnPoint = FindSpawnRecursively(root.transform, targetSpawnID);
            if (spawnPoint != null) break;
        }

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
        }
        else
        {
            Debug.LogWarning($"SpawnPoint '{targetSpawnID}' not found in scene '{targetChunk.SceneName}'");
        }
    }

    private Transform FindSpawnRecursively(Transform parent, string name)
    {
        if (parent.name == name) return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindSpawnRecursively(child, name);
            if (result != null) return result;
        }
        return null;
    }
}