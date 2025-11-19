using UnityEngine;

public class PromptManager : MonoBehaviour
{

    private Transform druidTransform;
    [SerializeField] private Vector2 promptCheckSize = new Vector2(2, 2);

  
    void Start()
    {
        druidTransform = GetComponent<Transform>();
    }

    void Update()
    {
        druidTransform.position = transform.position;

        Collider2D promptCheck = Physics2D.OverlapBox(druidTransform.position, promptCheckSize, 0f, LayerMask.GetMask("Dialogue"));

        if (promptCheck != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                IDialogue dialogue = promptCheck.gameObject.GetComponent<IDialogue>();
                if ( dialogue != null)
                {
                    if (dialogue.isInteracting == false)
                    {
                        dialogue.Interact();
                        Debug.Log("interacting with " + promptCheck.gameObject.name);
                    }
                }

            }
        }

    }
}
