using UnityEngine;

public abstract class DialogueTrigger : MonoBehaviour
{
    protected bool HasDialogue = false;
    protected Dialogue dialogue;
    private void Start()
    {
        if (!TryGetComponent(out dialogue))
        {
            Debug.Log("A DialogueTrigger could not find its Dialogue component. Make sure they are attched to the same gameObject.");
        }
        else HasDialogue = true;
    }
    protected abstract void Trigger();
}
