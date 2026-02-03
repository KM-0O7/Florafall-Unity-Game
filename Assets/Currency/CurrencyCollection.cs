using UnityEngine;

public class CurrencyCollection : MonoBehaviour
{
    [SerializeField] private int nutsGained = 5;
    CurrencyManager currencyManager;

    private void Start()
    {
        currencyManager = GameObject.FindGameObjectWithTag("NutText").GetComponent<CurrencyManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            currencyManager.gainBolts(nutsGained);
            Destroy(gameObject);
        }
    }


}
