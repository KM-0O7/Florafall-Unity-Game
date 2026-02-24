using UnityEngine;

public class CutsceneBars : MonoBehaviour
{
    Animator animator;
    public static CutsceneBars Instance;

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
       
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void CutsceneBarsStart()
    {
        animator.SetTrigger("CutsceneStart");
    }

    public void CutsceneBarsEnd()
    {
        animator.SetTrigger("CutsceneEnd");
    }

}
