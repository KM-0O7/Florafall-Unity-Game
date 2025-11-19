using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Speak : MonoBehaviour, IDialogue
{
    private bool interacting = false;
    public bool isInteracting => interacting;
    [SerializeField] private float typingSpeed = 0.1f;

    public string text = string.Empty;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private Animator dialogueAnimator;

    private Coroutine dialogueRoutine;

    public void Interact()
    {
        if (interacting == false)
        {
            
            interacting = true;
            dialogueRoutine = StartCoroutine(InteractingCoroutine());
        }
    }

    private void OnEnable()
    {
        if (dialogueBox == null)
            dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<Image>();

        if (textBox == null)
            textBox = GameObject.FindGameObjectWithTag("DialogueText").GetComponent<TextMeshProUGUI>();

        if (dialogueAnimator == null)
            dialogueAnimator = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<Animator>();


    }

    private IEnumerator InteractingCoroutine()
    {
        dialogueBox.enabled = true;
        textBox.text = text;
        textBox.maxVisibleCharacters = 0;
        dialogueAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < textBox.text.Length; i ++)
        {
            textBox.maxVisibleCharacters += 1;
            if (text[i] == '.' || text[i] == ',')
            {
                yield return new WaitForSeconds(typingSpeed + 0.5f);
            } 
            else
            {
                yield return new WaitForSeconds(typingSpeed);
            }  
        }
        
        yield return new WaitForSeconds(2f);
        textBox.text = "";
        dialogueAnimator.SetTrigger("Leave");
        yield return new WaitForSeconds(1f);
        dialogueBox.enabled = false;
        interacting = false;
    }

    private void OnDisable()
    {
      
        if (dialogueRoutine != null)
        {
            
            StopCoroutine(dialogueRoutine);
        }
           

        ResetDialogue();
    }

    private void ResetDialogue()
    { 
       if (dialogueAnimator != null)
        {
            dialogueAnimator.SetTrigger("Leave");
        }
        interacting = false;

        if (textBox != null)
        {
            textBox.text = "";
        }
          

        if (dialogueBox != null)
        {
            dialogueBox.enabled = false;
        }
            
    }
}
