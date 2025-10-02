using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [Header("Target Chunk")]
    public SceneField targetChunk;       // Drag your scene here

    private Animator fade;
    public string targetSpawnID;       // Name of spawn point in target scene
    private FollowPlayer camFollow;

    private void Start()
    {
        camFollow = Camera.main.GetComponent<FollowPlayer>();
        if (TransitionManager.Instance != null)
        {
            fade = TransitionManager.Instance.transitions;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(TeleportPlayer(other));
    }

    private IEnumerator TeleportPlayer(Collider2D player)
    {
        DruidFrameWork.Transitioning = true;
        fade.SetTrigger("Start");
        DruidFrameWork.canmove = false;
        yield return new WaitForSeconds(1.3f);

        DruidFrameWork druid = player.GetComponent<DruidFrameWork>();
        if (druid != null)
        {
            druid.spirits = druid.maxSpirits;
        }
        else
        {
            Debug.LogWarning("no framework lol");
        }
        DruidFrameWork.canmove = true;
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
        camFollow.SnapToTarget();
        DruidFrameWork.Transitioning = false;
        fade.SetTrigger("End");
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