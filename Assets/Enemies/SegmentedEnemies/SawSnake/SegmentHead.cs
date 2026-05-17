using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SegmentHead : MonoBehaviour
{
    public List<Segment> segments = new List<Segment>();
    public List<EnemyDamage> segmentHealth = new List<EnemyDamage>();
    private List<Vector2> path = new List<Vector2>();
    [SerializeField] private float segmentSpacing = 0.5f;
    [SerializeField] private float moveDistForRecord = 0.5f;
    private Vector2 lastRecordedPos;
    [SerializeField] private float detectionRange = 10f;
    private GameObject player;
    private Transform playerTransform;

    private void Start()
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
      
            path.Insert(0, transform.position);
            lastRecordedPos = transform.position;
        

        for (int i = 0; i < segments.Count; i++)
        {
            int pathIndex = Mathf.RoundToInt(i * segmentSpacing);

            if (pathIndex < path.Count)
            {
                segments[i].gameObject.transform.position = path[pathIndex];
            }
        }

        if (Vector2.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
        }

        for (int i = 0; i < segmentHealth.Count; i++)
        {
            if (segmentHealth[i].Dead)
            {
                Debug.Log("Segment Died");
                if (segments[i++] != null)
                {
                    var newHead = segments[i++].AddComponent<SegmentHead>();
                    newHead.segments.Clear();
                    segments.Remove(segments[i]);

                    for (int j = i++; j < segments.Count; j++)
                    {
                        newHead.segments.Add(segments[j]);
                        segments.Remove(segments[j]);
                    }
                }
            }
        }
    }
}