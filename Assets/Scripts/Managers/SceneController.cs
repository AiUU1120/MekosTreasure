using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] SceneFader sceneFaderPrefab;
    GameObject player;

    bool fadeFinished;

    [Header("===== SFX =====")]
    [SerializeField]
    private AudioData newGameSfx;
    [SerializeField]
    private AudioData loadScenceSfx;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    public void TransitionToDestination(PortalPoint portalPoint)
    {
        switch (portalPoint.portalType)
        {
            case PortalPoint.PortalType.SameSence:
                {
                    AudioManager.Instance.PlaySFX(loadScenceSfx);
                    StartCoroutine(Transition(SceneManager.GetActiveScene().name, portalPoint.destinationTag));
                    break;
                }
            case PortalPoint.PortalType.DifferentSence:
                {
                    AudioManager.Instance.PlaySFX(loadScenceSfx);
                    StartCoroutine(Transition(portalPoint.sceneName, portalPoint.destinationTag));
                    break;
                }
        }
    }

    IEnumerator Transition(string sceneName, DestinationPoint.DestinationTag destinationTag)
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        //TODO: 保存数据
        //SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)//跨场景传送
        {
            SaveManager.Instance.playerSaveData.Save(true);//保存游戏（不保存世界信息）
            yield return StartCoroutine(fader.FadeOut());
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.playerSaveData.Load(true);//加载游戏
            yield return StartCoroutine(fader.FadeIn());
            //Debug.Log("Different Portal");
            yield break;
        }
        else if (SceneManager.GetActiveScene().name == sceneName)//同场景传送
        {
            yield return StartCoroutine(fader.FadeOut(0.7f));
            player = GameManager.Instance.playStats.gameObject;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield return StartCoroutine(fader.FadeIn(0.7f));
            //Debug.Log("Same Portal");
            yield return null;
        }
    }

    private DestinationPoint GetDestination(DestinationPoint.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<DestinationPoint>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                Debug.Log("Find Destiation");
                return entrances[i];
            }
        }
        return null;
    }

    public void TransitionToMainMenu()//返回主菜单
    {
        AudioManager.Instance.PlaySFX(newGameSfx);
        StartCoroutine(LoadMainMenu());
    }

    public void TransitionToLoadGame()//加载存档场景
    {
        AudioManager.Instance.PlaySFX(loadScenceSfx);
        PlayerSaveData.Instance.loadSence();
        StartCoroutine(LoadLevel(SaveManager.Instance.playerSaveData.SceneName));
    }

    public void TransitionToFirstLevel()//加载第一个场景
    {
        AudioManager.Instance.PlaySFX(newGameSfx);
        SaveManager.Instance.playerSaveData.ResetStoryData();
        StartCoroutine(FirstLoadGame());
    }
    
    public void ChangeRoomFade()
    {
        StartCoroutine(ChangeRoom());
    }


    IEnumerator LoadLevel(string sceneName)//加载单一场景(从开始菜单）
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        if (sceneName != "")
        {
            yield return StartCoroutine(fader.FadeOut());
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab);
            SaveManager.Instance.playerSaveData.Load(false);//加载游戏
            yield return StartCoroutine(fader.FadeIn());
            fadeFinished = true;
            yield break;
        }
    }
    IEnumerator FirstLoadGame()//初次加载游戏
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeOut(1.5f));
        yield return SceneManager.LoadSceneAsync("RakuHome");
        yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
        SaveManager.Instance.playerSaveData.Save(false);//保存游戏
        yield return StartCoroutine(fader.FadeIn(5f));
        fadeFinished = true;
        yield break;
    }

    IEnumerator LoadMainMenu()//加载主菜单
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeOut());
        yield return SceneManager.LoadSceneAsync("MainMenu");
        yield return StartCoroutine(fader.FadeIn());
        fadeFinished = true;
        yield break;
    }

    IEnumerator ChangeRoom()//转换房间
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeWink(0.3f));
        fadeFinished = true;
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            //StartCoroutine(LoadMainMenu());//HACK：人物死亡退回主菜单，后续修改成选择菜单
        }
    }
}
