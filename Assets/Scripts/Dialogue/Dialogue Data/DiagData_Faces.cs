using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiagData_Faces", menuName = "Scriptable Objects/DiagData_Faces")]
public class DiagData_Faces : DialogueData
{
    [SerializeField] private Sprite face;

    public Sprite GetFaceSprite()
    {
        return face;
    }
}
