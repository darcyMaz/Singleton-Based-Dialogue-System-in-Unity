using System;
using UnityEngine;

public abstract class DialogueUI : MonoBehaviour
{
    public static DialogueUI instance { private set; get; }

    private bool SubscribedOnEnable = false;

    protected DialogueData CurrentLine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnEnable()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnStartDialogue += StartDialogue;
            DialogueManager.instance.OnNextDialogue += NextDialogue;
            DialogueManager.instance.OnEndDialogue += EndDialogue;
            SubscribedOnEnable = true;
        }
        else Debug.Log("DialogueUI failed to subscribe OnEnable. This is normal if there is not another warning on Start().");
    }
    
    private void OnDisable()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnStartDialogue -= StartDialogue;
            DialogueManager.instance.OnNextDialogue -= NextDialogue;
            DialogueManager.instance.OnEndDialogue -= EndDialogue;
        }
    }
    private void Start()
    {
        if (DialogueManager.instance != null)
        {
            if (!SubscribedOnEnable)
            {
                DialogueManager.instance.OnStartDialogue += StartDialogue;
                DialogueManager.instance.OnNextDialogue += NextDialogue;
                DialogueManager.instance.OnEndDialogue += EndDialogue;
                SubscribedOnEnable = true;
            }
        }
        else Debug.Log("DialogueUI could not find the DialogueManager instance on Start.");
    }

    protected void StartDialogue(DialogueData dd)
    {
        CurrentLine = dd;
        OpenDialogueUI();
    }
    protected void NextDialogue(DialogueData dd)
    {
        CurrentLine = dd;
        UpdateDialogueUI();
    }
    protected void EndDialogue()
    {
        CloseDialogueUI();
    }

    protected abstract void OpenDialogueUI();
    protected abstract void UpdateDialogueUI();
    protected abstract void CloseDialogueUI();
}
