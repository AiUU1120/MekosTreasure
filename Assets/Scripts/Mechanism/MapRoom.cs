using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//此脚本用于进出房间时相机的切换
public class MapRoom : MonoBehaviour
{
    public GameObject virtualCam;
    CinemachineVirtualCamera virtualCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        virtualCamera = virtualCam.GetComponent<CinemachineVirtualCamera>();
        if (other.CompareTag("Player") && !other.isTrigger)//HACK:将该处脚本放在子物体中避免重复调用
        {
            //Debug.Log("Trigger");
            virtualCamera.Follow = other.transform;  
            virtualCam.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(false);
        }
    }
}
