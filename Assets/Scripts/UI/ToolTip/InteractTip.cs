using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class InteractTip : MonoBehaviour
{
    [SerializeField]
    private Canvas interactTipCanvas;
    [SerializeField]
    private RectTransform interactTipRectTransform;
    [SerializeField]
    private float transitionTime;
    [SerializeField]
    private AudioData tipSfx;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenTip();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CloseTip();
        }
    }

    private void OpenTip()
    {
        interactTipCanvas.enabled = true;
        interactTipRectTransform.localScale = Vector3.zero;
        if (interactTipRectTransform != null)
        {
            interactTipRectTransform.DOScale(Vector3.one, transitionTime);
        }
        AudioManager.Instance.PlaySFX(tipSfx);
    }

    private void CloseTip()
    {
        StartCoroutine(CloseTipIEnumerator());
    }

    IEnumerator CloseTipIEnumerator()
    {
        if (interactTipRectTransform != null)
        {
            interactTipRectTransform.DOScale(Vector3.zero, transitionTime);
        }
        yield return new WaitForSeconds(transitionTime);
        interactTipCanvas.enabled = false;
        yield break;
    }
}
