using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
    [SerializeField] protected string line;

    public string GetText()
    {
        return line;
    }
     
}
