using System.Collections;
using System.Collections.Generic;
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
    private List<bool> animatingTetherTargets = new List<bool>();
    [SerializeField] private ParticleSystem tetherBreak;
    [SerializeField] private float tetherRetractTime = 0.5f;

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
                if (animatingTetherTargets[i] == false)
                {
                    activeTethers[i].SetPosition(0, druidtransform.position);
                    activeTethers[i].SetPosition(1, tetherTargets[i].position);
                }
              
                //fading from dist
                float distance = Vector2.Distance(druidtransform.position, tetherTargets[i].position);

                float d = Mathf.Clamp01(distance / maxTetherDistance);
                Gradient gradient = activeTethers[i].colorGradient;

                GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

                float expo = Mathf.Pow(d, 5f);

                for (int j = 0; j < alphaKeys.Length; j++)
                {
                    alphaKeys[j].alpha = Mathf.Lerp(1, 0.05f, expo);
                }

                gradient.alphaKeys = alphaKeys;
                activeTethers[i].colorGradient = gradient;

                if (distance > maxTetherDistance)
                {
                    Debug.Log(tetherTargets[i] + " tether is too far! Breaking Tether");

                    // Degrow plant
                    DeGrowPlant(tetherTargets[i]);
                }
            }
        }

        //---- HIGHLIGHTS ----

        if (!UI.dead && !DruidFrameWork.isTransformed && !DruidFrameWork.inCutscene)
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
                    IDamageAble damage = hit.collider.GetComponent<IDamageAble>();
                    IGrowablePlant plant = hit.collider.GetComponent<IGrowablePlant>();

                    if ((plant != null && plant.WaterGrown == false)|| (enemy != null && damage != null && !damage.Dead && !enemy.CantGrow))
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

        // ---- UNGROW ALL ACTIVE PLANTS ----
        if (Input.GetKeyDown(KeyCode.F) && !DruidFrameWork.inCutscene)
        {
            if (DruidFrameWork.isTransformed == false && DruidLedgeClimb.isMantled == false)
            {
                DeGrowAllPlants();
            }
        }

        if (!UI.dead && !DruidFrameWork.inCutscene)
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
                                if (!plant.IsGrown && !plant.CanDie && UI.spirits >= plant.spiritCost && !plant.WaterGrown)
                                {
                                    // Grow the plant
                                    plant.Grow();
                                    Debug.Log("Growing" + hit.collider.name);
                                    StartCoroutine(GrowPlant(hit.collider.transform));
                                }
                                else if (plant.CanDie && plant.IsGrown) DeGrowPlant(hit.collider.transform);
                            }
                        }

                        //---- GROWABLE ENEMY FRAMEWORK
                        else
                        {
                            IGrowableEnemy enemy = hit.collider.GetComponent<IGrowableEnemy>();
                            IDamageAble damage = hit.collider.GetComponent<IDamageAble>();
                            if (enemy != null)
                            {
                                float distance = Vector2.Distance(druidtransform.position, hit.collider.transform.position);
                                if (distance <= maxTetherDistance - 2)
                                {
                                    if (!damage.Dead)
                                    {
                                        if (!enemy.IsGrown && !enemy.CanDie && UI.spirits > 0 && !enemy.CantGrow)
                                        {
                                            // Grow the enemy
                                            enemy.Grow();
                                            Debug.Log("Growing" + hit.collider.name);
                                            StartCoroutine(GrowPlant(hit.collider.transform));
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
     * GrowPlant - Call to grow the plant/enemy at specified transform'
     * DeGrowAllPlants - Call to degrow all plants/enemies
     */

    public void DeGrowAllPlants()
    {
        for (int i = tetherTargets.Count - 1; i >= 0; i--)
        {
            if (activeTethers[i] != null && tetherTargets[i] != null)
            {
                DeGrowPlant(tetherTargets[i]);
            }
        }
    }

    public IEnumerator RemoveTether(Transform plantTransform)
    {
        int index = tetherTargets.IndexOf(plantTransform);

        if (index != -1)
        {
            Debug.Log("Removing " + activeTethers[index].name + " Tether!");
            animator.SetTrigger("Grow");
            animator.SetBool("Growing", true);
            Invoke("StopGrowing", 0.2f);

            animatingTetherTargets[index] = true;
            //Retraction
            float t = 0;
            while (t <= tetherRetractTime)
            {
                yield return null;
                Vector2 pos2 = activeTethers[index].GetPosition(1);
                Vector2 pos1 = activeTethers[index].GetPosition(0);
                pos2 = Vector2.Lerp(pos2, pos1, t / tetherRetractTime);
                activeTethers[index].SetPosition(1, pos2);
                activeTethers[index].SetPosition(0, druidtransform.position);
                t += Time.deltaTime;
            }
            Destroy(activeTethers[index].gameObject);
            activeTethers.RemoveAt(index);
            tetherTargets.RemoveAt(index);
            animatingTetherTargets.RemoveAt(index);
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
                StartCoroutine(RemoveTether(planttransform));  
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
                    StartCoroutine(RemoveTether(planttransform));
                }
            }
        }
    }


    private IEnumerator GrowPlant(Transform plantTransform) //call this function add tether to growed plant
    {
        animator.SetTrigger("Grow");
        animator.SetBool("Growing", true);
        Invoke("StopGrowing", 0.2f);

        IGrowablePlant plant = plantTransform.GetComponent<IGrowablePlant>();
        if (plant != null)
        {
            LineRenderer tetherclone = Instantiate(tether);
            tetherclone.positionCount = 2;
            tetherclone.useWorldSpace = true;
            activeTethers.Add(tetherclone);
            tetherTargets.Add(plantTransform);
            animatingTetherTargets.Add(true);
            int index = tetherTargets.IndexOf(plantTransform);
            UI.spirits -= plant.spiritCost;

            float t = 0;
            while (t <= tetherRetractTime)
            {
                yield return null;
                Vector2 pos2 = druidtransform.position;
                pos2 = Vector2.Lerp(pos2, plantTransform.position, t / tetherRetractTime);
                activeTethers[index].SetPosition(1, pos2);
                activeTethers[index].SetPosition(0, druidtransform.position);
                t += Time.deltaTime;
            }
            tetherclone.SetPosition(0, druidtransform.position);
            tetherclone.SetPosition(1, plantTransform.position);
            animatingTetherTargets[index] = false;
        }
        else //enemies
        {
            IGrowableEnemy enemy = plantTransform.GetComponent<IGrowableEnemy>();
            if (enemy != null)
            {
                LineRenderer tetherclone = Instantiate(tether);
                tetherclone.positionCount = 2;
                tetherclone.useWorldSpace = true;
                activeTethers.Add(tetherclone);
                tetherTargets.Add(plantTransform);
                animatingTetherTargets.Add(false);
                UI.spirits -= enemy.spiritCost;
                int index = tetherTargets.IndexOf(plantTransform);
                
                float t = 0;
                while (t <= tetherRetractTime)
                {
                    yield return null;
                    Vector2 pos2 = druidtransform.position;
                    pos2 = Vector2.Lerp(pos2, plantTransform.position, t / tetherRetractTime);
                    activeTethers[index].SetPosition(1, pos2);
                    activeTethers[index].SetPosition(0, druidtransform.position);
                    t += Time.deltaTime;
                }
                tetherclone.SetPosition(0, druidtransform.position);
                tetherclone.SetPosition(1, plantTransform.position);
                animatingTetherTargets[index] = false;
            }
        }
    }
}