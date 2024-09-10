using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerPanelController : MonoBehaviour
{
    public void OpenPanel()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(1f, 1f, 1f), 1f);
    }
}
