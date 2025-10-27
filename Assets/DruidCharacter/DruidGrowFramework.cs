using System.Collections.Generic;
using UnityEngine;

public class DruidGrowFramework : MonoBehaviour
{

    /* DRUIDGROWFRAMWORK
     * This script handles all growable plants and tethers
     * Handles bear attacks
     * Handes enemy growth
     */


    //tethers
    public LineRenderer tether;
    public float maxTetherDistance;
    public Transform druidtransform;

    private List<LineRenderer> activeTethers = new List<LineRenderer>();
    private List<Transform> tetherTargets = new List<Transform>();

    //UI and Scripts
    DruidUI UI;
    Animator animator;
    DruidFrameWork druid;

    /* START
     * Handles all components
     */
    void Start()
    {
        //components
        druid = GetComponent<DruidFrameWork>();
        animator = GetComponent<Animator>();
        UI = GetComponent<DruidUI>();
        
    }


    void Update()
    {
        //updatetethers via list
        for (int i = 0; i < activeTethers.Count; i++)
        {
            if (activeTethers[i] != null && tetherTargets[i] != null)
            {
                activeTethers[i].SetPosition(0, druidtransform.position);
                activeTethers[i].SetPosition(1, tetherTargets[i].position);

                float distance = Vector2.Distance(druidtransform.position, tetherTargets[i].position);

                if (distance > maxTetherDistance)
                {
                    Debug.Log("Tether too far, breaking...");

                    // Degrow plant
                    DeGrowPlant(tetherTargets[i]);
                }
            }
        }

 
        if (!UI.dead)
        {
            //If Clicked
            if (Input.GetMouseButtonDown(0))
            {
                if (DruidFrameWork.isTransformed == false)
                {
                    //find mouse pos
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("GrowPlants", "GrowEnemy"));
                    if (hit.collider != null)
                    {
                        IGrowablePlant plant = hit.collider.GetComponent<IGrowablePlant>();
                        if (plant != null)
                        {
                            float distance = Vector2.Distance(druidtransform.position, hit.collider.transform.position);
                            if (distance <= maxTetherDistance - 2)
                            {
                                if (!plant.IsGrown && UI.spirits > plant.spiritCost)
                                {
                                    // Grow the plant
                                    plant.Grow();
                                    Debug.Log("Growing");
                                    growplant(hit.collider.transform);
                                }
                                else if (plant.CanDie)
                                {
                                    // Degrow the plant
                                    DeGrowPlant(hit.collider.transform);
                                    animator.SetTrigger("Grow");
                                }
                            }
                        }

                        //Enemies
                        else
                        {
                            IGrowableEnemy enemy = hit.collider.GetComponent<IGrowableEnemy>();
                            if (enemy != null)
                            {
                                float distance = Vector2.Distance(druidtransform.position, hit.collider.transform.position);
                                if (distance <= maxTetherDistance - 2)
                                {
                                    if (!enemy.Dead)
                                    {
                                        if (!enemy.IsGrown && UI.spirits > 0)
                                        {
                                            // Grow the enemy
                                            enemy.Grow();
                                            Debug.Log("Growing");
                                            growplant(hit.collider.transform);
                                        }
                                        else if (enemy.CanDie)
                                        {
                                            // Degrow the enemy
                                            DeGrowPlant(hit.collider.transform);
                                            animator.SetTrigger("Grow");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (DruidFrameWork.isTransformed) //transformations
                {
                    if (!DruidFrameWork.bearattackcd) //bear attack
                    {
                        if (DruidFrameWork.canjump)
                        {
                            druid.BearAttack();
                        }
                    }
                }
            }
        }
    }
       

    //call this function to remove active tether
    public void RemoveTether(Transform plantTransform)
    {
        // Find which index in the list this plant is at
        int index = tetherTargets.IndexOf(plantTransform);

        if (index != -1) // if found
        {
            Destroy(activeTethers[index].gameObject);

            activeTethers.RemoveAt(index);
            tetherTargets.RemoveAt(index);
        }
    }

    //call function to kill plant
    private void DeGrowPlant(Transform planttransform)
    {
        IGrowablePlant plant = planttransform.GetComponent<IGrowablePlant>();
        if (plant != null)
        {
            UI.spirits+= plant.spiritCost;
            plant.Die();
            RemoveTether(planttransform);
        }
        else
        {
            //enemy ungrow
            IGrowableEnemy enemy = planttransform.GetComponent<IGrowableEnemy>();
            if (enemy != null)
            {
                UI.spirits += 3;

                enemy.Die();
                RemoveTether(planttransform);
            }
        }
    }

    private void growplant(Transform plantTransform) //call this function add tether to growed plant
    {
        IGrowablePlant plant = plantTransform.GetComponent<IGrowablePlant>();
        if (plant != null)
        {
            LineRenderer tetherclone = Instantiate(tether);
            tetherclone.positionCount = 2;
            tetherclone.SetPosition(0, druidtransform.position);
            tetherclone.SetPosition(1, plantTransform.position);
            tetherclone.useWorldSpace = true;
            activeTethers.Add(tetherclone);
            tetherTargets.Add(plantTransform);

            UI.spirits -= plant.spiritCost;
            animator.SetTrigger("Grow");
        }
        else //enemies
        {
            IGrowableEnemy enemy = plantTransform.GetComponent<IGrowableEnemy>();
            if (enemy != null)
            {
                LineRenderer tetherclone = Instantiate(tether);
                tetherclone.positionCount = 2;
                tetherclone.SetPosition(0, druidtransform.position);
                tetherclone.SetPosition(1, plantTransform.position);
                tetherclone.useWorldSpace = true;
                activeTethers.Add(tetherclone);
                tetherTargets.Add(plantTransform);

                UI.spirits -= 3;
                animator.SetTrigger("Grow");
            }
        }
    }
}

