using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;

    private PlayerInput input;

    private bool canTalk = false;

    [SerializeField]
    private bool isInteract;

    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (canTalk && !isInteract)
        {
            OpenDialogue();
            canTalk = false;
        }

        if (canTalk && isInteract && input.Interact)
        {
            OpenDialogue();
        }
    }

    private void OpenDialogue()
    {
        //DialogueUI.Instance.UpdateDialogueData(currentData);
        //DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
        InputDeviceDetection.GameplayUIController.Instance.UpdateDialogueData(currentData);
        InputDeviceDetection.GameplayUIController.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = false;
        }
    }
}
