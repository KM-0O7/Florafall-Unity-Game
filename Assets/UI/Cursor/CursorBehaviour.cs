using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{

    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.visible = false;
    }

    
    void Update()
    {
        Vector3 cursorPos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(cursorPos);
        gameObject.transform.position = new Vector3(worldPos.x, worldPos.y, 10);
        Cursor.visible = false;

        if (Input.GetMouseButton(0))
        {
            animator.SetTrigger("Click");
        } else
        { 
            animator.SetTrigger("Release");
        }

    }
}
