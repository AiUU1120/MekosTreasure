using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型单例
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }


    protected virtual void OnDestroy()//销毁
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    public static bool IsInitialized//判断是否单例已经生成
    {
        get { return instance != null; }
    }
}
