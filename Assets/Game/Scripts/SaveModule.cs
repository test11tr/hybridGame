using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class SaveModule : MonoBehaviour
{
    void Awake()
    {        
        loadData();
    }

    void Start()
    {
        StartCoroutine(AutoSaveCoroutine());
    }

    public SaveInfo saveInfo;

    public void saveData()
    {
        Debug.Log("Data saved!");
        var jsonData = JsonUtility.ToJson(saveInfo);
        var dataPath = Application.persistentDataPath + "/saveData.json";
        System.IO.File.WriteAllText(dataPath, jsonData);
    }

    public void loadData()
    {
        var dataPath = Application.persistentDataPath + "/saveData.json";
        if (System.IO.File.Exists(dataPath))
        {
            var jsonData = System.IO.File.ReadAllText(dataPath);
            saveInfo = JsonUtility.FromJson<SaveInfo>(jsonData);
            Debug.Log("Data loaded!");
        }
        else
        {
            saveInfo = new SaveInfo();
        }
    }

    public void resetData()
    {
        saveInfo = new SaveInfo();
        saveData();
        Debug.Log("Data reset!");
    }

    IEnumerator AutoSaveCoroutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(30); 
            saveData(); 
        }
    }


    void OnApplicationQuit()
    {
        saveData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            saveData();
        }
    }

    [Serializable]
    public class SaveInfo
    {
        public int coinCount;
        public int gemCount;
        public int woodAmount;
        public int stoneAmount;
        public int characterCurrentHealth;
    }
}
