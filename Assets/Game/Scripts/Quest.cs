using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

[Serializable]
public class Quest : MonoBehaviour {
    public int ID;
    public string Title;
    public string Description;
    public bool IsCompleted { get; protected set; }

    public virtual void StartQuest()
    {
    }
    public virtual void CompleteQuest() {
        IsCompleted = true;
        GameManager.Instance.questManager.CompleteQuest(this);
    }
}