using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscMenuCanvas : MonoBehaviour
{
    [SerializeField]
    private Button returnButton;
    [SerializeField]
    private Button exitButton;

    private void OnEnable()
    {
        returnButton.onClick.AddListener(OnReturnButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        returnButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    private void OnReturnButtonClick()
    {
        InputDeviceDetection.GameplayUIController.Instance.CloseEscMenuCanvas();
    }
    private void OnExitButtonClick()
    {
        Time.timeScale = 1f;
        SceneController.Instance.TransitionToMainMenu();
    }
}
