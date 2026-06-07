using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DiagTrig_HoverClick : DialogueTrigger
{
    private InputSystem_Actions _actions;
    private InputAction interact;

    [SerializeField] private string TriggerZoneTagComparison = "Player";
    private bool IsInTrigger = false;

    // In this example, there can be a UI prompt accessible via a public function.
    [SerializeField] private Image promptBG;
    [SerializeField] private TextMeshProUGUI promptText;
    private bool CanPrompt = true;
    [SerializeField] private string PromptName;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        interact = _actions.Player.Interact;
        interact.performed += Pressed;
        interact.Enable();
    }

    private void OnDisable()
    {
        interact.performed -= Pressed;
        interact.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TriggerZoneTagComparison))
        {
            IsInTrigger = true;
            OpenPrompt(PromptName);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(TriggerZoneTagComparison))
        {
            IsInTrigger = false;
            ClosePrompt();
        }
    }

    private void Pressed(InputAction.CallbackContext context)
    {
        if (context.performed && IsInTrigger && CanPrompt)
        {
            Trigger();
        }
    }

    protected override void Trigger()
    {
        if (HasDialogue)
        {
            PromptSwitch(false);
            dialogue.TryStartDialogue(); 
        }
    }

    private void OpenPrompt(string Name)
    {
        if (!CanPrompt) return;
        promptBG.color = new Color(promptBG.color.r, promptBG.color.g, promptBG.color.b, 1);
        promptText.text = "Hold E to interact with " + Name + ".";
    }
    private void ClosePrompt()
    {
        promptBG.color = new Color(promptBG.color.r, promptBG.color.g, promptBG.color.b, 0);
        promptText.text = "";
    }
    private void PromptSwitch(bool flip)
    {
        CanPrompt = flip;
        if (!CanPrompt) ClosePrompt();
    }
}
