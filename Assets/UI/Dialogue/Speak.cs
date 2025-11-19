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
    private Rigidbody2D druidRig;
    private Animator druidAnimator;

    private bool textOn = false;
    private bool skippedText = false;

    private Coroutine dialogueRoutine;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            druidAnimator = player.GetComponent<Animator>();
            druidRig = player.GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (textOn == true && skippedText == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                skippedText = true;
            }
        }
    }

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
        DruidFrameWork.canjump = false;
        DruidFrameWork.canmove = false;
        druidRig.linearVelocity = new Vector2(0, 0);

        druidAnimator.SetFloat("XVelo", 0f);
        textOn = true;
        dialogueBox.enabled = true;
        textBox.text = text;
        textBox.maxVisibleCharacters = 0;
        dialogueAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < textBox.text.Length; i++)
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

            if (skippedText == true)
            {
                textBox.maxVisibleCharacters = text.Length;
                break;
            }
        }

        DruidFrameWork.canjump = true;
        DruidFrameWork.canmove = true;
        textOn = false;
        yield return new WaitForSeconds(0.1f);
        skippedText = false;
        yield return new WaitForSeconds(0.9f);
        textBox.text = "";
        dialogueAnimator.SetTrigger("Leave");
        yield return new WaitForSeconds(1f);
        textBox.maxVisibleCharacters = 0;
        dialogueBox.enabled = false;
        yield return new WaitForSeconds(1.5f);
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