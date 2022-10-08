using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHouse : MonoBehaviour
{
    PlayerInput input;

    private bool canOpen;
    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (canOpen && input.Interact && !InputDeviceDetection.GameplayUIController.Instance.isUI)
        {
            InputDeviceDetection.GameplayUIController.Instance.OpenCatHouseCanvas();
        }
        if (input.goBack && InputDeviceDetection.GameplayUIController.Instance.isCatHouse && !InputDeviceDetection.GameplayUIController.Instance.isSecondaryMenu)
        {
            InputDeviceDetection.GameplayUIController.Instance.CloseCatHouseCanvas();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("can shop");
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = false;
        }
    }
}
