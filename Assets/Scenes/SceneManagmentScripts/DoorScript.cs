using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [Header("Target Chunk")]
    public SceneField targetChunk;

    private Animator fade;
    public string targetSpawnID;
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

        if (DruidFrameWork.Transitioning) return;
        StartCoroutine(TeleportPlayer(other));
    }

    private IEnumerator TeleportPlayer(Collider2D player)
    {
        DruidFrameWork.Transitioning = true;
        fade.SetTrigger("Start");
        DruidUI UI = player.GetComponent<DruidUI>();
        DruidFrameWork.canmove = false;

        //colours
        if (gameObject.tag == "MountainDoor")
        {
            UI.circleWipe.color = new Color(78f / 255f, 104f / 255f, 154f / 255f);
        }
        else if (gameObject.tag == "FloraForestDoor")
        {
            UI.circleWipe.color = new Color(29f / 255f, 142f / 255f, 47f / 255f);
        }

        yield return new WaitUntil(() =>
        fade.GetCurrentAnimatorStateInfo(0).IsName("CircleWipeExposed"));

        DruidFrameWork druid = player.GetComponent<DruidFrameWork>();

        if (druid != null)
        {
            if (!DruidFrameWork.isTransformed)
            {
                UI.spirits = UI.maxSpirits;
            }
        }
        else
        {
            Debug.LogWarning("no framework lol");
        }

        ChunkLoader.Instance.EnterChunk(targetChunk.SceneName);

        Scene targetScene = SceneManager.GetSceneByName(targetChunk.SceneName);
        while (!targetScene.isLoaded)
            yield return null;

        Transform spawnPoint = null;
        foreach (GameObject root in targetScene.GetRootGameObjects())
        {
            spawnPoint = FindSpawnRecursively(root.transform, targetSpawnID);
            if (spawnPoint != null) break;
        }

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            Rigidbody2D playerRig = player.GetComponent<Rigidbody2D>();
            playerRig.linearVelocity = new Vector2(0, 0);
            fade.SetTrigger("End");
            DruidFrameWork.Transitioning = false;
            DruidFrameWork.canmove = true;
            camFollow.SnapToTarget();
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