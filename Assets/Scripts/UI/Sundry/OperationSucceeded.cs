using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationSucceeded : MonoBehaviour
{
    [SerializeField]
    private Button OperationSucceededButton;

    private void OnEnable()
    {
        OperationSucceededButton.onClick.AddListener(OnOperationSucceededButtonClick);
    }

    private void OnDisable()
    {
        OperationSucceededButton.onClick.RemoveAllListeners();
    }

    private void OnOperationSucceededButtonClick()
    {
        InputDeviceDetection.GameplayUIController.Instance.OperationSuccess();
    }
}
