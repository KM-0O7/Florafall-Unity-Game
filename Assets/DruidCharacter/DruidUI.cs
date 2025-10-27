using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DruidUI : MonoBehaviour
{
    public Image[] spiritimages;

    
    public Sprite fullSpirit;
    public Sprite emptySpirit;
    public int maxSpirits; //change to set max spirits max is 8
    public int spirits; //current spirits
    public Image circleWipe;
    private Transform spawnPoint; //current spawnpoint;
    public float health = 5;
    public float MaxHealth = 5;
    public bool dead = false;
    public GameObject druid;
    [SerializeField] private Animator deathScreen;
    private bool waitCycle = false;
    public string spawnSceneName;
    private Animator druidanims;


    void Start()
    {
        druidanims = GetComponent<Animator>();
        health = MaxHealth;
    }

    void Update()
    {
        for (int i = 0; i < spiritimages.Length; i++) //set spirit UI can change in Inspector
        {
            if (i < spirits)
            {
                spiritimages[i].sprite = fullSpirit;
            }
            else
            {
                spiritimages[i].sprite = emptySpirit;
            }

            if (i < maxSpirits)
            {
                spiritimages[i].enabled = true;
            }
            else
            {
                spiritimages[i].enabled = false;
            }
        }

        if (health <= 0)
        {
            if (dead == false)
            {
                if (!waitCycle)
                {
                    StartCoroutine(DeathScreenCycle());
                }
            }  
        }

        if (dead)
        {
            if (waitCycle)
            {
               
                    StartCoroutine(RespawnCycle());
              
            }  
        }
    }

    private IEnumerator DeathScreenCycle()
    {
        health = 0;
        waitCycle = true;
        deathScreen.SetTrigger("Death");
        druidanims.SetTrigger("Death");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(0.9f);
        dead = true;
    }

    private IEnumerator RespawnCycle()
    {
        waitCycle = false;
        deathScreen.SetTrigger("Respawn");
        yield return new WaitForSeconds(0.1f);

        druidanims.SetTrigger("Respawn");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(spawnSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }


        yield return null;
        spawnPoint = GameObject.FindWithTag("RespawnPoint")?.transform;

        health = MaxHealth;
        dead = false;
        spirits = maxSpirits;

    
        if (spawnPoint != null)
        {
            druid.transform.position = spawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No spawnPoint found in scene!");
        }
    }
}
