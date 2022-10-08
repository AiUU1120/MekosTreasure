using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("===== Basic Element =====")]
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMP_Text mainText;

    [SerializeField]
    private Canvas dialogueCanvas;
    [SerializeField]
    private RectTransform standPanelTransform;
    [SerializeField]
    private RectTransform dialogueTransform;

    [Header("===== Data =====")]
    [SerializeField]
    private DialogueData_SO currentData;

    private int currentIndex = 0;

    public void UpdateDialogueData(DialogueData_SO data)
    {
        currentData = data;
        currentIndex = 0;
    }
    
    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialogueCanvas.enabled = true;
        if(currentIndex == 0)
        {
            standPanelTransform.anchoredPosition = new Vector2(-550f, -20f);
            standPanelTransform.DOAnchorPos(new Vector2(0f, -20f), 0.3f);
            dialogueTransform.anchoredPosition = new Vector2(0f, -700f);
            dialogueTransform.DOAnchorPos(new Vector2(0f, -353f), 0.3f);
        }

        if(piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;
        }
        mainText.text = "";
        mainText.text = piece.text;
    }
}
