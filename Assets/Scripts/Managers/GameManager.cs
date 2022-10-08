using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerMeko;
    public CharacterStats playStats;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(GameObject player)
    {
        playerMeko = player;
        playStats = player.GetComponent<CharacterStats>();
        SaveManager.Instance.playerSaveData.LoadStoryAgain();
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()//获取出生点
    {
        foreach(var item in FindObjectsOfType<DestinationPoint>())
        {
            if(item.destinationTag == DestinationPoint.DestinationTag.START)
            {
                return item.transform;
            }
        }
        return null;
    }
}
