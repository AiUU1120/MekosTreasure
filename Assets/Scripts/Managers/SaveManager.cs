using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : Singleton<SaveManager>
{
    //string sceneName = "";//保存玩家所在场景

    //public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }
    public PlayerSaveData playerSaveData;

    protected override void Awake()
    {
        base.Awake();
        playerSaveData = GetComponent<PlayerSaveData>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    SceneController.Instance.TransitionToMainMenu();
        //}
        #region playerPrefs
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Debug.Log("Save");
        //    SavePlayerData();
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    Debug.Log("Load");
        //    LoadPlayerData();
        //}
        #endregion
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    //var data = LoadFromJson<CharacterData_SO>(GameManager.Instance.playStats.characterData.name);
        //    //GameManager.Instance.playStats.characterData.canDash = data;
        //    Load();
        //}
    }

    private void Save()
    {
        SaveByJson(GameManager.Instance.playStats.characterData, GameManager.Instance.playStats.characterData.name);//保存玩家ScriptableObject
    }
    private void Load()
    {
        LoadFromJsonSO(GameManager.Instance.playStats.characterData.name);
    }


    #region PlayerPrefs
    //public void SavePlayerData()
    //{
    //    Save(GameManager.Instance.playStats.characterData, GameManager.Instance.playStats.characterData.name);
    //}

    //public void LoadPlayerData()
    //{
    //    Load(GameManager.Instance.playStats.characterData, GameManager.Instance.playStats.characterData.name);
    //}

    //public void Save(Object data, string key)
    //{
    //    var jsonData = JsonUtility.ToJson(data, true);
    //    Debug.Log(jsonData);
    //    PlayerPrefs.SetString(key, jsonData);
    //    PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
    //    PlayerPrefs.Save();
    //}

    //public void Load(Object data, string key)
    //{
    //    if (PlayerPrefs.HasKey(key))
    //    {
    //        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
    //    }
    //}
    #endregion


    #region JSON
    public void SaveByJson(object data, string saveFileName)
    {
        var json = JsonUtility.ToJson(data, true);
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);
        try
        {
#if UNITY_EDITOR
            Debug.Log($"Successfully saved data to {path}.");
            Debug.Log(json);
#endif
        }
        catch (System.Exception exception)
        {
#if UNITY_EDITOR
            Debug.LogError($"Fail to save data to {path}.\n{exception}");
#endif
        }
    }

    public T LoadFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        try
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                //Debug.Log("Here");
                var data = JsonUtility.FromJson<T>(json);
                return data;
            }
            return default;
        }
        catch (System.Exception exception)
        {
#if UNITY_EDITOR
            Debug.LogError($"Fail to load data from {path}.\n{exception}");
#endif
            return default;
        }
    }
    public string LoadFromJsonSO(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        try
        {
            var json = File.ReadAllText(path);
            return json;
            //JsonUtility.FromJsonOverwrite(json, GameManager.Instance.playStats.characterData);

        }
        catch (System.Exception exception)
        {
#if UNITY_EDITOR
            Debug.LogError($"Fail to load data from {path}.\n{exception}");
#endif
            return default;
        }
    }
    #endregion

    #region Delete
    public void DeleteSaveFile(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        try
        {
            File.Delete(path);
        }
        catch (System.Exception exception)
        {
#if UNITY_EDITOR
            Debug.LogError($"Fail to delete data in {path}.\n{exception}");
#endif
        }
    }
    #endregion
}
