using UnityEngine;
using UnityEngine.UI;
public class DruidUI : MonoBehaviour
{
    public Image[] spiritimages;

    
    public Sprite fullSpirit;
    public Sprite emptySpirit;
    public int maxSpirits; //change to set max spirits max is 8
    public int spirits; //current spirits
    public Image circleWipe;
    void Start()
    {
        
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

    }
}
