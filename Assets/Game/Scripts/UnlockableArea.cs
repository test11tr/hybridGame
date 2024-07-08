using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockableArea : MonoBehaviour
{
    public string areaName;
    [DisplayWithoutEdit()] public bool isLocked = true;
    public GameObject popupUI;
    public GameObject Gate;
    public Collider collider;
    public ParticleSystem unlockEffect;
    public TMP_Text questDescriptionText;
    public Image progressBar;
    public TMP_Text progressText;

    private void Start()
    {
        popupUI.SetActive(false);
        bool isAreaUnlocked = GameManager.Instance.saveModule.CheckIfAreaUnlocked(areaName);
        if (isAreaUnlocked)
        {
            isLocked = false;
            Gate.SetActive(false);
            collider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isLocked)
        {
            showUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isLocked)
        {
            closeUI();
        }
    }

    public void showUI()
    {
        popupUI.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        popupUI.SetActive(true);
        popupUI.transform.DOScale(.75f, .5f);
    }

    public void closeUI()
    {
        popupUI.transform.DOScale(0.0f, .5f).OnComplete(() => {
                popupUI.SetActive(false);
        });
    }

    public void unlockArea()
    {
        unlockEffect.Play();
        isLocked = false;
        Gate.SetActive(false);
        popupUI.SetActive(false);
        collider.enabled = false;
        GameManager.Instance.saveModule.UpdateUnlockedAreas(areaName, true);
    }
}
