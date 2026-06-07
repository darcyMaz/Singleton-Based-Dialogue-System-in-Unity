using System;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private bool SubscribedOnEnable = false;

    // When a Dialogue component appears or dissapears in the scene, tell the game of this change.
    public static event Action <Dialogue> OnDialogueEnable;
    public static event Action <Dialogue> OnDialogueDisable;

    // When this Dialogue is triggered to start.
    public event Action<IEnumerable<DialogueData>> OnDialogueTriggered;

    // Dialogue Information. Each DialogueData is a line of Dialogue.
    [SerializeField] private List<DialogueData> dialogue;

    public void TryStartDialogue()
    {
        OnDialogueTriggered?.Invoke(GetDialogue());
    }

    private void Start()
    {
        if (!SubscribedOnEnable && DialogueManager.instance != null)
        {
            OnDialogueEnable?.Invoke(this);
        }
        else Debug.Log("A Dialogue could not find DialogueManager instance on Start. This may be normal if it finds it OnEnable.");
    }

    private void OnEnable()
    {
        if (DialogueManager.instance != null)
        {
            OnDialogueEnable?.Invoke(this);
            SubscribedOnEnable = true;
        }
        else Debug.Log("A Dialogue could not find DialogueManager instance OnEnable. This may be normal if it finds it at Start.");
    }

    private void OnDisable()
    {
        if (DialogueManager.instance != null) OnDialogueDisable?.Invoke(this);
    } 

    private IEnumerable<DialogueData> GetDialogue()
    {
        foreach (DialogueData dd in dialogue)
        {
            yield return dd;
        }
    }
}
