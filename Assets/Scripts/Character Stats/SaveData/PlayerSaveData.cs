using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaveData : Singleton<PlayerSaveData>
{
    protected override void Awake()
    {
        base.Awake();
    }

    private string sceneName = "";
    private Vector3 playerPosition;
    private Vector3 playerScale;

    public bool isStart = false;
    public bool isGameTutorial = false;
    public bool isFelkoReact = false;
    public bool isFelkoBeat = false;
    public bool isYuriTownReact = false;
    [System.Serializable]
    class SaveData
    {
        //世界数据
        public string sceneName;//保存玩家所在场景
        public Vector3 playerPosition;//保存玩家位置
        public Vector3 playerScale;//保存玩家朝向
    }
    [System.Serializable]
    class StoryData
    {
        //剧情数据
        public bool isStart;//开局剧情
        public bool isGameTutorial;//获取操作教程
        public bool isFelkoReact;//遇见菲儿可
        public bool isFelkoBeat;//击败菲儿可
        public bool isYuriTownReact;//到达尤里镇
    }

    public string SceneName => sceneName;
    public Vector3 PlayerPosition => playerPosition;


    const string PLAYER_DATA_FILE_NAME = "PlayerData.sav";
    const string STORY_DATA_FILE_NAME = "StoryData.sav";
    const string MEKO_DATA_FILE_NAME = "MEKOData.sav";
    const string ATTACK_DATA_FILE_NAME = "AttackData.sav";
    const string BAG_DATA_FILE_NAME = "BagData.sav";
    const string ACTION_DATA_FILE_NAME = "AtionData.sav";


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Save(false);
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Load(false);
        //}
    }

    public void Save(bool isTrans)
    {
        SaveByJson(isTrans);
    }

    public void Load(bool isGaming)
    {
        LoadFromJson(isGaming);
    }

    void SaveByJson(bool isTrans)
    {
        if (!isTrans)
        {
            SaveManager.Instance.SaveByJson(SavingData(), PLAYER_DATA_FILE_NAME);//保存世界信息
        }
        SaveManager.Instance.SaveByJson(SavingStoryData(), STORY_DATA_FILE_NAME);
        SaveManager.Instance.SaveByJson(GameManager.Instance.playStats.characterData, MEKO_DATA_FILE_NAME);//保存玩家数据ScriptableObject
        SaveManager.Instance.SaveByJson(GameManager.Instance.playStats.attackData, ATTACK_DATA_FILE_NAME);//保存玩家攻击数据ScriptableObject
        SaveManager.Instance.SaveByJson(InventoryManager.Instance.inventoryData, BAG_DATA_FILE_NAME);//保存背包数据ScriptableObject
        SaveManager.Instance.SaveByJson(InventoryManager.Instance.actionData, ACTION_DATA_FILE_NAME);//保存动作栏数据ScriptableObject
    }

    void LoadFromJson(bool isGaming)
    {
        if (!isGaming)
        {
            var saveData = SaveManager.Instance.LoadFromJson<SaveData>(PLAYER_DATA_FILE_NAME);
            Debug.Log(saveData.playerPosition);
            LoadData(saveData);//读取世界信息
        }
        var saveStoryData = SaveManager.Instance.LoadFromJson<StoryData>(STORY_DATA_FILE_NAME);
        Debug.Log(saveStoryData.isStart);
        LoadStory(saveStoryData);
        JsonUtility.FromJsonOverwrite(SaveManager.Instance.LoadFromJsonSO(MEKO_DATA_FILE_NAME), GameManager.Instance.playStats.characterData);//读取玩家数据ScriptableObject
        JsonUtility.FromJsonOverwrite(SaveManager.Instance.LoadFromJsonSO(ATTACK_DATA_FILE_NAME), GameManager.Instance.playStats.attackData);//读取攻击数据ScriptableObject
        JsonUtility.FromJsonOverwrite(SaveManager.Instance.LoadFromJsonSO(BAG_DATA_FILE_NAME), InventoryManager.Instance.inventoryData);//读取背包数据ScriptableObject
        JsonUtility.FromJsonOverwrite(SaveManager.Instance.LoadFromJsonSO(ACTION_DATA_FILE_NAME), InventoryManager.Instance.actionData);//读取动作栏数据ScriptableObject
        InventoryManager.Instance.inventoryUI.ReFreshUI(false);//刷新背包
        InventoryManager.Instance.actionUI.ReFreshUI(true);//刷新动作栏
    }

    public void LoadStoryAgain()
    {
        var saveStoryData = SaveManager.Instance.LoadFromJson<StoryData>(STORY_DATA_FILE_NAME);
        Debug.Log("Load Again");
        Debug.Log(saveStoryData.isStart);
        LoadStory(saveStoryData);
    }

    SaveData SavingData()//返回SaveData数据
    {
        var saveData = new SaveData();
        //场景
        sceneName = SceneManager.GetActiveScene().name;
        saveData.sceneName = sceneName;
        playerPosition = GameManager.Instance.playerMeko.transform.position;
        saveData.playerPosition = playerPosition;
        playerScale = GameManager.Instance.playerMeko.transform.localScale;
        saveData.playerScale = playerScale;

        return saveData;
    }

    StoryData SavingStoryData()
    {
        var saveStoryData = new StoryData();
        //故事进度
        saveStoryData.isStart = isStart;
        saveStoryData.isGameTutorial = isGameTutorial;
        saveStoryData.isFelkoReact = isFelkoReact;
        saveStoryData.isFelkoBeat = isFelkoBeat;
        saveStoryData.isYuriTownReact = isYuriTownReact;

        return saveStoryData;
    }

    public void ResetStoryData()
    {
        isStart = false;
        isGameTutorial = false;
        isFelkoReact = false;
        isFelkoBeat = false;
        isYuriTownReact = false;
        SaveManager.Instance.SaveByJson(SavingStoryData(), STORY_DATA_FILE_NAME);
    }

    private void LoadData(SaveData saveData)
    {
        //场景
        sceneName = saveData.sceneName;
        playerPosition = saveData.playerPosition;
        playerScale = saveData.playerScale;


        GameManager.Instance.playerMeko.transform.position = playerPosition;
        GameManager.Instance.playerMeko.transform.localScale = playerScale;
    }

    private void LoadStory(StoryData saveStoryData)
    {
        //故事进度
        isStart = saveStoryData.isStart;
        isGameTutorial = saveStoryData.isGameTutorial;
        isFelkoReact = saveStoryData.isFelkoReact;
        isFelkoBeat = saveStoryData.isFelkoBeat;
        isYuriTownReact = saveStoryData.isYuriTownReact;
    }

    public void loadSence()
    {
        var saveData = SaveManager.Instance.LoadFromJson<SaveData>(PLAYER_DATA_FILE_NAME);
        if (saveData != null)
        {
            Debug.Log(saveData.playerPosition);
            sceneName = saveData.sceneName;
        }
    }
}
