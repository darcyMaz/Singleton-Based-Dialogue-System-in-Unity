using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI_Ex1 : DialogueUI
{
    [SerializeField] private Image face;
    [SerializeField] private Image textBG;
    [SerializeField] private TextMeshProUGUI diagText;

    protected override void CloseDialogueUI()
    {
        textBG.color = new Color(textBG.color.r, textBG.color.g, textBG.color.b, 0);
        diagText.text = "";
        face.enabled = false;
    }

    protected override void OpenDialogueUI()
    {
        UpdateDialogueUI();

        textBG.color = new Color(textBG.color.r, textBG.color.g, textBG.color.b, 1);
        face.enabled = true;
    }

    protected override void UpdateDialogueUI()
    {
        try
        {
            DiagData_Faces currentLine = (DiagData_Faces)CurrentLine;
            diagText.text = currentLine.GetText();
            face.sprite = currentLine.GetFaceSprite();
        }
        catch (Exception e)
        {
            CloseDialogueUI();
            Debug.LogError("A DialogueUI_Ex1 component tried to convert its super class' (dialogueUI) CurrentLine variable from the super class DialogueData to DiagData_Faces. This failure means that you are either using the wrong DialogueData class or the wrong DialogueUI class.");
            Debug.LogError(e);
        }
    }
}
