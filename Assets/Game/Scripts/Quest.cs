using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

[Serializable]
public class Quest : MonoBehaviour {
    public int ID;
    public string Description;
    public bool IsCompleted;

    public virtual void StartQuest()
    {
    }
    
    public virtual void CompleteQuest() {
        IsCompleted = true;
        GameManager.Instance.questManager.CompleteQuest(this);
    }
}