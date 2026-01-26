using UnityEngine;
using System.Collections;

public class RootSword : MonoBehaviour, IGrowablePlant
{
    private bool growDB = false;
    public bool IsGrown => growDB;
    private bool canDie = false;
    public bool CanDie => canDie;
    public int spiritCost => 1;
    [SerializeField] private GameObject swordObject;
    private GameObject swordClone;
    private GameObject druid;
    [SerializeField] private float swordSpeed = 2f;
    private DruidGrowFramework druidGrow;

    private LineRenderer tether;
    private LineRenderer tetherClone;
    [SerializeField] private float swordDistance = 5;

    private void Start()
    {
        druid = GameObject.FindGameObjectWithTag("Player");
        druidGrow = druid.GetComponent<DruidGrowFramework>();
        tether = GameObject.FindGameObjectWithTag("PlantTether").GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (CanDie)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            swordClone.transform.position = Vector2.Lerp(swordClone.transform.position, mousePos, swordSpeed * Time.deltaTime);
            if (tetherClone != null)
            {
                tetherClone.SetPosition(0, druid.transform.position);
                tetherClone.SetPosition(1, swordClone.transform.position);
            }

            if (Vector2.Distance(druid.transform.position, swordClone.transform.position) > swordDistance) druidGrow.DeGrowPlant(transform);
        }
    }

    public void Grow()
    {
        if (!growDB && !canDie) StartCoroutine(GrowCycle());
    }

    public void Die()
    {
        if (growDB && canDie) StartCoroutine(DieCycle());
    }

    private IEnumerator GrowCycle()
    {
        growDB = true;
        swordClone = Instantiate(swordObject);

        tetherClone = Instantiate(tether);
        tetherClone.positionCount = 2;

        tetherClone.useWorldSpace = true;

        while (Vector2.Distance(druid.transform.position, swordClone.transform.position) > swordDistance - 2)
        {
            swordClone.transform.position = Vector2.Lerp(swordClone.transform.position, druid.transform.position, swordSpeed * Time.deltaTime);
            tetherClone.SetPosition(0, druid.transform.position);
            tetherClone.SetPosition(1, swordClone.transform.position);
            yield return null;
        }

        canDie = true;
    }

    private IEnumerator DieCycle()
    {
        canDie = false;
        growDB = false;
        yield return null;
        Destroy(tetherClone);
        Destroy(swordClone);
    }
}