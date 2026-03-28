using UnityEngine;
public class Prompter: MonoBehaviour
{
    [SerializeField] private string objectTag;
    private Animator animator;
    private bool alreadyInteracted = false;
    void Start()
    {
        animator = GameObject.FindGameObjectWithTag(objectTag).GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyInteracted) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Show");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (alreadyInteracted) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            alreadyInteracted = true;
            animator.SetTrigger("Hide");
        }
    }
}
