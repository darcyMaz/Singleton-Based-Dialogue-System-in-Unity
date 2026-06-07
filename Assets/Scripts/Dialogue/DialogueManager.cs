using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { private set; get; }

    public event Action<DialogueData> OnStartDialogue;
    public event Action<DialogueData> OnNextDialogue;
    public event Action OnEndDialogue;

    private List<DialogueData> CurrentDialogue = new List<DialogueData>();
    private int DialogueIndex = 0;

    private bool IsDialogueOngoing = false;
    private bool IsDialogueLocked = false;

    private InputSystem_Actions isa;
    private InputAction next;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        isa = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        Dialogue.OnDialogueEnable += AddDialogue;
        Dialogue.OnDialogueDisable += RemoveDialogue;

        next = isa.Player.Next;
        next.performed += NextDialogue;
        next.Enable();
    }

    private void OnDisable()
    {
        Dialogue.OnDialogueEnable -= AddDialogue;
        Dialogue.OnDialogueDisable -= RemoveDialogue;
    }

    private void AddDialogue(Dialogue d)
    {
        d.OnDialogueTriggered += StartDialogue;
    }
    private void RemoveDialogue(Dialogue d)
    {
        d.OnDialogueTriggered -= StartDialogue;
    }

    private void StartDialogue(IEnumerable<DialogueData> lines)
    {
        // If Dialogues are not locked and if there is not an active Dialogue.
        if (!IsDialogueOngoing && !IsDialogueLocked)
        {
            IsDialogueOngoing = true;

            CurrentDialogue.Clear();
            DialogueIndex = 0;
            foreach (DialogueData dd in lines)
            {
                CurrentDialogue.Add(dd);
            }

            // If the input IEnumerable returned an empty enumeration of DialogueData, simply do nothing.
            if (CurrentDialogue.Count <= 0)
            {
                return;
            }

            // Otherwise, start the Dialogue.
            OnStartDialogue?.Invoke(CurrentDialogue[DialogueIndex]);
        }
    }
    private void NextDialogue(InputAction.CallbackContext context)
    {
        if (context.performed && IsDialogueOngoing)
        {
            DialogueIndex++;

            // The Dialogue has concluded.
            if (DialogueIndex == CurrentDialogue.Count)
            {
                IsDialogueOngoing = false;
                OnEndDialogue?.Invoke();
            }
            // The Dialogue is not finished yet.
            else
            {
                OnNextDialogue?.Invoke(CurrentDialogue[DialogueIndex]);
            }
        }
    }
}
