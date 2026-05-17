using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SegmentHead : MonoBehaviour
{
    public List<Segment> segments = new List<Segment>();
    public List<EnemyDamage> segmentHealth = new List<EnemyDamage>();
    private List<Vector2> path = new List<Vector2>();
    private float segmentSpacing = 4f;
    private float moveDistForRecord = 0.1f;
    private Vector2 lastRecordedPos;
    [SerializeField] private float detectionRange = 10f;
    private GameObject player;
    private Transform playerTransform;

    private void Start()
    {
        StartUpSegment();
    }
    public void StartUpSegment()
    {
        lastRecordedPos = transform.position;
        path.Add(transform.position);
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.GetComponent<Transform>();
        }
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, lastRecordedPos) >= moveDistForRecord)
        {
            path.Insert(0, transform.position);
            lastRecordedPos = transform.position;
        }

        for (int i = 0; i < segments.Count; i++)
        {
            int pathIndex = Mathf.RoundToInt(i * segmentSpacing);
            segments[i].segmentPosition = i;
            if (pathIndex < path.Count)
            {
                if (segments[i].currentOwner == gameObject.GetComponent<SegmentHead>())
                {
                    segments[i].gameObject.transform.position = path[pathIndex];
                }
            }
        }

        //---- BEHAVIOUR ----
        if (Vector2.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
        }
    }


    //---- SPLITTING LOGIC ----

    /* Void Split
     * Split is a helper function that allows the segmented enemy to split
     * Split will destroy the segment at split pos and separate the entire segemented enemy into multiple parts
     * If there is no segments after the splitpos, it will simply destroy the segment
     */
    public void Split(int splitPos)
    {
        if (splitPos < 0 || splitPos >= segments.Count)
        {
            Debug.LogError("Invalid split position");
            return;
        }

        Debug.Log("Split at " + splitPos);
        Segment segmentToBeDestroyed = segments[splitPos];
        int newHeadIndex = splitPos + 1;

        if (newHeadIndex >= segments.Count)
        {
            Destroy(segmentToBeDestroyed.gameObject);
            segments.RemoveAt(splitPos);
            return;
        }
       
        var headOfNewPack = segments[newHeadIndex].gameObject;

        var newHead = headOfNewPack.AddComponent<SegmentHead>();
        newHead.StartUpSegment();

        for (int i = newHeadIndex; i < segments.Count; i++)
        {
            segments[i].currentOwner = newHead;
            newHead.segments.Add(segments[i]);
            newHead.segmentHealth.Add(segments[i].gameObject.GetComponent<EnemyDamage>());
        }

        for (int i = segments.Count - 1; i >= newHeadIndex; i--)
        {
            segmentHealth.RemoveAt(i);
            segments.RemoveAt(i);
        }
        Destroy(segments[splitPos].gameObject);
        segments.Remove(segments[splitPos]);
    }
}