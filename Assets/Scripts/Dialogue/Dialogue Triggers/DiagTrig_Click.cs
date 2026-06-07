using UnityEngine.InputSystem;

public class DiagTrig_Click : DialogueTrigger
{
    private InputSystem_Actions _actions;
    private InputAction interact;

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

    private void Pressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Trigger();
        }
    }

    protected override void Trigger()
    {
        if (HasDialogue)
        {
            dialogue.TryStartDialogue();
        }
    }
}
