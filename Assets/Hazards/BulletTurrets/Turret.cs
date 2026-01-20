using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float bulletDamage = 1f;
    private SpriteRenderer sprite;
    private float direction = 1f; 

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
   
    void Update()
    {
        
    }
}
