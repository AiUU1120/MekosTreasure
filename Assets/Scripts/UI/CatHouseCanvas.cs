using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatHouseCanvas : MonoBehaviour
{
    [SerializeField]
    private Button healButton;
    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private Button loadButton;

    private void OnEnable()
    {
        healButton.onClick.AddListener(OnHealButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
    }

    private void OnDisable()
    {
        healButton.onClick.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();
    }

    private void OnHealButtonClick()
    {
        GameManager.Instance.playStats.ApplyHealth(999);
        InputDeviceDetection.GameplayUIController.Instance.OpenSuccessCanvas();
    }
    private void OnSaveButtonClick()
    {
        PlayerSaveData.Instance.Save(false);
        InputDeviceDetection.GameplayUIController.Instance.OpenSuccessCanvas();
    }
    private void OnLoadButtonClick()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
}
