using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该脚本用于场景的渐入渐出
public class SceneFader : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField] float fadeOutDuration;//淡出时间
    [SerializeField] float fadeInDuration;//淡入时间
    [SerializeField]
    private AudioData fadeOutSfx;
    [SerializeField]
    private AudioData fadeInSfx;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
        
    }

    public IEnumerator FadeOutIn()//先淡出再淡入
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }
    public IEnumerator FadeOutIn(float time)//先淡出再淡入
    {
        yield return FadeOut(time);
        yield return FadeIn(time);
    }

    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }
    public IEnumerator FadeOut()
    {
        
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeOutDuration;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float time)
    {
       
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        Destroy(gameObject);
    }
    public IEnumerator FadeIn()
    {
        
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeInDuration;
            yield return null;
        }
        Destroy(gameObject);
    }

    public IEnumerator FadeWink(float time)//只淡出不淡入
    {
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        Destroy(gameObject);
    }
}
