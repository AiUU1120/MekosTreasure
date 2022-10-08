using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogueEventType { Normal, ShopA, ShopB, ShopC, FelkoSpawn, FelkoBeat }

[System.Serializable]
public class DialoguePiece
{
    public string ID;

    public Sprite image;

    public string speakerName;

    [TextArea]
    public string text;

    public bool isFromNpc;
    public bool isEndNpc;

    public DialogueEventType eventType;

    public List<DialogueOption> options = new List<DialogueOption>();
}
