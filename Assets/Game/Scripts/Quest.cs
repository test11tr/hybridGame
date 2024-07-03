using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : MonoBehaviour {
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