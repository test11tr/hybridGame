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
        GameStart();
    }

    void Start()
    {
        StartCoroutine(AutoSaveCoroutine());
    }

    public SaveInfo saveInfo;

    public void GameStart()
    {  
        var dataPath = Application.persistentDataPath + "/saveData.json";
        if (System.IO.File.Exists(dataPath))
        {
            print("Found a saved a Data");
            loadData();
        }else
        {
            saveInfo = new SaveInfo();
            Debug.Log("Created a new Data!");
        }
    }

    public void saveData()
    {
        var jsonData = JsonUtility.ToJson(saveInfo);
        var dataPath = Application.persistentDataPath + "/saveData.json";
        System.IO.File.WriteAllText(dataPath, jsonData);
        Debug.Log("Data succesfully saved!");
    }

    public void loadData()
    {
        var dataPath = Application.persistentDataPath + "/saveData.json";
        var jsonData = System.IO.File.ReadAllText(dataPath);
        saveInfo = JsonUtility.FromJson<SaveInfo>(jsonData);
        Debug.Log("Data succesfully loaded!");
    }

    public void resetData()
    {
        var dataPath = Application.persistentDataPath + "/saveData.json";
        if (System.IO.File.Exists(dataPath))
        {
            System.IO.File.Delete(dataPath);
            Debug.Log("Data deleted!");
        }else
        {
            Debug.Log("No data found to delete!");
        }
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
        if(saveInfo.isFirstRun == true)
        {
            saveInfo.isFirstRun = false;
        }
        saveData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            saveData();
        }
    }

    public bool CheckIsHaveSave()
    {
        var dataPath = Application.persistentDataPath + "/saveData.json";
        if (System.IO.File.Exists(dataPath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Serializable]
    public class SaveInfo
    {
        [Header("SaveCheck")]
        public bool isFirstRun=true;
        [Header("Wallet")]
        public int coinCount;
        public int gemCount;
        public int woodAmount;
        public int stoneAmount;
        [Header("VirtualWallet")]
        public int virtualCoin;
        public int virtualGem;
        public int virtualWood;
        public int virtualStone;
        [Header("Character & Incrementals")]
        public float movementSpeedCurrentValue;
        public int movementSpeedCurrentLevel;
        public float damageCurrentValue;
        public int damageCurrentLevel;
        public float criticalChanceCurrentValue;
        public float criticalChanceCurrentLevel;
        public float criticalMultiplierCurrentValue;
        public float criticalMultiplierCurrentLevel;
        public float AttackRangeCurrentValueMelee;
        public float AttackRangeCurrentValueRange;
        public int AttackRangeCurrentLevel;
        public float detectorRange;
        public float attackSpeedCurrentValueMelee;
        public float attackSpeedCurrentValueRange;
        public int attackSpeedCurrentLevel;
        public float healthCurrentValue;
        public float healthCurrentLevel;
        public float characterCurrentHealth;
        public float splashDamageMultiplier;
        [Header("Level/Experience")]
        public int characterCurrentExperience;
        public int characterCurrentLevel;
        public int characterExperienceToNextLevel;
        [Header("Quests")]
        public List<Quest> quests;
    }
}
