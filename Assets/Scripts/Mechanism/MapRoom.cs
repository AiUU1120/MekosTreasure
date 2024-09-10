using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//�˽ű����ڽ�������ʱ������л�
public class MapRoom : MonoBehaviour
{
    public GameObject virtualCam;
    CinemachineVirtualCamera virtualCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        virtualCamera = virtualCam.GetComponent<CinemachineVirtualCamera>();
        if (other.CompareTag("Player") && !other.isTrigger)//HACK:���ô��ű������������б����ظ�����
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
