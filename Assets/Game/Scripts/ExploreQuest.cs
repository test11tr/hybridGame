using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Shapes;

public class ExploreQuest : Quest 
{
    public int RequiredAmount = 1;
    public int CurrentAmount = 0;
    public bool hasHotspot;
    public CinemachineVirtualCamera hotspotCamera;
    public BoxCollider hotspotCollider;

    public override void StartQuest() {
        base.StartQuest();
        Debug.Log($"{Title}: {Description}. Explore Area.");
        SetButtonListener();
        UpdateUI();
    }

    private void SetButtonListener()
    {
        hotspotCamera.Priority = 11;
        DelayHelper.DelayAction(3.5f, () => {
            hotspotCamera.Priority = 1;
        });

        
        if(hasHotspot)
        {
            GameManager.Instance.questManager.magnifyingGlass.gameObject.SetActive(true);
            GameManager.Instance.questManager.questButton.onClick.AddListener(MoveToHotspot);
        }
    }

    public void UpdateUI() {
        GameManager.Instance.questManager.questBg.color = new Color32(34, 34, 34, 178);
        GameManager.Instance.questManager.questDescriptionText.text = Description;
        GameManager.Instance.questManager.progressBar.fillAmount = (float)CurrentAmount / RequiredAmount;
        GameManager.Instance.questManager.progressText.text = $"{CurrentAmount}/{RequiredAmount}";
        GameManager.Instance.questManager.completeTick.SetActive(false);
        GameManager.Instance.questManager.questSlot.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            CurrentAmount = 1;
            UpdateUI();
            CheckGoal();
        }
    }

    private void CheckGoal()
    {
        if (CurrentAmount >= RequiredAmount && !IsCompleted)
        {
            GameManager.Instance.questManager.questBg.color = new Color(0.62f, 0.91f, 0.33f);
            GameManager.Instance.questManager.questButton.onClick.RemoveAllListeners();
            GameManager.Instance.questManager.questButton.onClick.AddListener(CompleteQuest);
            GameManager.Instance.questManager.completeTick.SetActive(true);
        }
    }

    public override void CompleteQuest() {
        base.CompleteQuest();
    }

    public void MoveToHotspot()
    {
        hotspotCamera.Priority = 11;
        DelayHelper.DelayAction(3.5f, () => {
            hotspotCamera.Priority = 1;
        });
    }
}
