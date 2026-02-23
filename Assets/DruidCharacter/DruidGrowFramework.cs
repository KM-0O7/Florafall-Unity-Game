
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DruidGrowFramework : MonoBehaviour
{
    /* DRUIDGROWFRAMWORK
     * This script handles all growable plants and tethers
     * Handles bear attacks
     * Handes enemy growth
     * Handles highlighting plants
     */

    //tethers
    public LineRenderer tether;

    public float maxTetherDistance;
    public Transform druidtransform;

    private List<LineRenderer> activeTethers = new List<LineRenderer>();
    private List<Transform> tetherTargets = new List<Transform>();
    [SerializeField] private ParticleSystem tetherBreak;

    //HIGHLIGHTS
    private Transform lastHoveredObject;

    private Material lastHoveredMaterial;

    //UI and Scripts
    private DruidUI UI;

    private Animator animator;
    private DruidFrameWork druid;

    /* START
     * Handles all components
     */

    private void Start()
    {
        //components
        druid = GetComponent<DruidFrameWork>();
        animator = GetComponent<Animator>();
        UI = GetComponent<DruidUI>();
    }

    /* UPDATE
     * Inlcudes Highlighting for plants
     * Includes Plant Growth and death
     * Manages all tethers in the list
     */

    private void Update()
    {
        //---- TETHER LOOP -----
        for (int i = 0; i < activeTethers.Count; i++)
        {
            if (activeTethers[i] != null && tetherTargets[i] != null)
            {
                activeTethers[i].SetPosition(0, druidtransform.position);
                activeTethers[i].SetPosition(1, tetherTargets[i].position);

                float distance = Vector2.Distance(druidtransform.position, tetherTargets[i].position);

                if (distance > maxTetherDistance)
                {
                    Debug.Log(tetherTargets[i] + " tether is too far! Breaking Tether");

                    // Degrow plant
                    DeGrowPlant(tetherTargets[i]);
                }
            }
        }

        //---- HIGHLIGHTS ----

        if (!UI.dead && !DruidFrameWork.isTransformed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("GrowPlants", "GrowEnemy"));

            if (lastHoveredObject != null && lastHoveredMaterial != null) //reset to avoid multiple instances
            {
                lastHoveredMaterial.SetFloat("_On_Off", 0);
                lastHoveredObject = null;
                lastHoveredMaterial = null;
            }

            if (hit.collider != null)
            {
                Transform target = hit.collider.transform;
                float distance = Vector2.Distance(druidtransform.position, target.position);

                if (distance < maxTetherDistance - 2)
                {
                    IGrowableEnemy enemy = hit.collider.GetComponent<IGrowableEnemy>();
                    IGrowablePlant plant = hit.collider.GetComponent<IGrowablePlant>();

                    if (plant != null || enemy != null)
                    {
                        if ((plant.CanDie == false && plant.IsGrown == false) || (plant.CanDie == true && plant.IsGrown == true) ||
                            (enemy.CanDie == false && enemy.IsGrown == false) || (enemy.CanDie == true && enemy.IsGrown == true))
                        {
                            SpriteRenderer growableRender = hit.collider.GetComponent<SpriteRenderer>();

                            if (growableRender != null)
                            {
                                if (!growableRender.material.name.EndsWith("(Instance)"))
                                {
                                    growableRender.material = new Material(growableRender.material);
                                }

                                Debug.Log(hit.collider.name + "Highlighted");
                                growableRender.material.SetFloat("_On_Off", 1);

                                lastHoveredObject = target;
                                lastHoveredMaterial = growableRender.material;
                            }
                        }
                    }
                }
            }
        }

        // ---- UNGROW ALL ACTIVE PLANTS ----
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (DruidFrameWork.isTransformed == false && DruidLedgeClimb.isMantled == false)
            {
                for (int i = tetherTargets.Count - 1; i >= 0; i--)
                {
                    if (activeTethers[i] != null && tetherTargets[i] != null)
                    {
                        DeGrowPlant(tetherTargets[i]);
                    }
                }
            }
        }

        if (!UI.dead)
        {
            // ---- GROW FRAMEWORK ----
            if (Input.GetMouseButtonDown(0))
            {
                if (DruidFrameWork.isTransformed == false && DruidLedgeClimb.isMantled == false)
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
                                if (!plant.IsGrown && !plant.CanDie && UI.spirits >= plant.spiritCost)
                                {
                                    // Grow the plant
                                    plant.Grow();
                                    Debug.Log("Growing" + hit.collider.name);
                                    growplant(hit.collider.transform);
                                }
                                else if (plant.CanDie && plant.IsGrown) DeGrowPlant(hit.collider.transform);
                            }
                        }

                        //---- GROWABLE ENEMY FRAMEWORK
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
                                        if (!enemy.IsGrown && !enemy.CanDie && UI.spirits > 0)
                                        {
                                            // Grow the enemy
                                            enemy.Grow();
                                            Debug.Log("Growing" + hit.collider.name);
                                            growplant(hit.collider.transform);
                                        }
                                        else if (enemy.CanDie && enemy.IsGrown) DeGrowPlant(hit.collider.transform);
                                    }
                                }
                            }
                        }
                    }
                }

                //---- TRANFORMATION ABILITIES ----
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

    /* FUNCTIONS
     * RemoveTether - Call to remove tether at specified transformb
     * DeGrowPlant - Call to degrow the plant/enemy at specified transform
     * GrowPlant - Call to grow the plant/enemy at specified transform
     */

    public void RemoveTether(Transform plantTransform)
    {
        // Find which index in the list this plant is at
        int index = tetherTargets.IndexOf(plantTransform);

        if (index != -1)
        {
            animator.SetTrigger("Grow");
            animator.SetBool("Growing", true);
            Invoke("StopGrowing", 0.2f);
            Destroy(activeTethers[index].gameObject);

            activeTethers.RemoveAt(index);
            tetherTargets.RemoveAt(index);
        }
    }

    //call to stop the grow anim
    private void StopGrowing()
    {
        animator.SetBool("Growing", false);
    }

    //call function to kill plant
    public void DeGrowPlant(Transform planttransform)
    {
        IGrowablePlant plant = planttransform.GetComponent<IGrowablePlant>();
        IGrowableEnemy enemy = planttransform.GetComponent<IGrowableEnemy>();
        
        if (plant != null)
        {
            if (plant.CanDie && plant.IsGrown)
            {
                UI.spirits += plant.spiritCost;
                plant.Die();
                RemoveTether(planttransform);
                tetherBreak.Emit(3);
            }
        }
        else
        {
            //enemy ungrow
            if (enemy != null)
            {
                if (enemy.CanDie && enemy.IsGrown)
                {
                    UI.spirits += enemy.spiritCost;
                    enemy.Die();
                    RemoveTether(planttransform);
                    tetherBreak.Emit(3);
                } 
            }
        }
    }

    private void growplant(Transform plantTransform) //call this function add tether to growed plant
    {
        animator.SetTrigger("Grow");
        animator.SetBool("Growing", true);
        Invoke("StopGrowing", 0.2f);

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

                UI.spirits -= enemy.spiritCost;
            }
        }
    }
}