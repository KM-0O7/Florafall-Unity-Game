
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;
    public float nuts = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void gainBolts(float nuts)
    {
        this.nuts += nuts;
    }
    public void looseBolts(float nuts)
    {
        this.nuts -= nuts;
    }
    public void setBolts(float nuts)
    {
        this.nuts = nuts;
    }

}
