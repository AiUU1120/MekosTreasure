using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopIndex { A, B, C };
public class Shop : MonoBehaviour
{
    PlayerInput input;
    [SerializeField]
    private ShopIndex shopIndex;

    private bool canShop;
    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (canShop && input.Interact && !InputDeviceDetection.GameplayUIController.Instance.isUI)
        {
            Debug.Log("shop");
            //InputDeviceDetection.GameplayUIController.Instance.OpenShop(shopIndex);
        }
        if (input.goBack && InputDeviceDetection.GameplayUIController.Instance.isShop)
        {
            InputDeviceDetection.GameplayUIController.Instance.CloseShop();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("can shop");
            canShop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canShop = false;
        }
    }
}
