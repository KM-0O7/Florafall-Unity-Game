using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    public Animator transitions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (transitions == null)
        {
            transitions = GetComponent<Animator>();
        }
    }
}