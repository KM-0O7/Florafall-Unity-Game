using System.ComponentModel;
using TreeEditor;
using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float swoopSpeed = 2f;
    [SerializeField] private float playerDetectionDistance = 2f;

    [SerializeField] private float bobbingSpeed = 0f;
    [SerializeField] private float bobbingHeight = 0.5f;

    [SerializeField] private float wallDetectionDistance = 1f;
    [SerializeField] private Vector2 wallDetectionBoxSize;
    private Animator animator;
    private bool isSwooping = false;
    private bool playerInSight = false;
    private Transform enemyTransform;
    private Transform playerTransform;



    void Start()
    {
        animator = GetComponent<Animator>();
        enemyTransform = GetComponent<Transform>();
    

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.gameObject.GetComponent<Transform>();
        }

    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        if (distance < playerDetectionDistance)
        {
            playerInSight = true;
        } else
        {
            playerInSight = false;
        }

        //Calculate bobbing up and down with a sine wave
        var bobbingOffset = Time.deltaTime * bobbingSpeed;

        float yBobbing = Mathf.Sin(bobbingOffset) * bobbingHeight;

        Vector3 pos = transform.position;
        pos.y += yBobbing * Time.deltaTime;  
        transform.position = pos;

        //Detect if enemy is near a wall
        Collider2D wall = Physics2D.OverlapBox(transform.position, wallDetectionBoxSize, 0f, LayerMask.GetMask("Ground"));
        if (wall != null)
        {

        }
    }
}
