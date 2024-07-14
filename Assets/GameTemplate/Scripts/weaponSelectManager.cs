using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements.Experimental;
using System.Runtime.InteropServices;
using System;

public class weaponSelectManager : MonoBehaviour
{
    public enum WeaponType
    {
        MeleeAttack,
        RangedAttack,
        RotatingBlades
    }

    [Foldout("Character Modules (Gets Automatic)", foldEverything = true, styled = true, readOnly = true)]
    public CharacterStatsModule statsModule;
    public CharacterControlManager playerManager;
    public SaveModule saveModule;
    [Foldout("Button References", foldEverything = true, styled = true, readOnly = false)]
    public Button swordSelectButton;
    public TMP_Text swordSelectText;
    public Button StaffSelectButton;
    public TMP_Text StaffSelectText;
    public Button BowSelectButton;
    public TMP_Text BowSelectText;

    void Start()
    {
        if(statsModule == null)
        {
            statsModule = GameManager.Instance.player.statsModule;
        }
        if(playerManager == null)
        {
            playerManager = GameManager.Instance.player;
        }
        if(saveModule == null)
        {
            saveModule = GameManager.Instance.saveModule;
        }

        UpdateUI();
        SetButtons();
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void SetButtons()
    {
        swordSelectButton.onClick.AddListener(SelectMeleeWeapon);
        StaffSelectButton.onClick.AddListener(SelectStaff);
        BowSelectButton.onClick.AddListener(SelectBow);
    }

    public void UpdateUI()
    {
        if(playerManager.isMeleeAttack)
        {
            swordSelectText.text = "SELECTED";
            swordSelectButton.interactable = false;
            StaffSelectText.text = "SELECT";
            StaffSelectButton.interactable = true;
            BowSelectText.text = "SELECT";
            BowSelectButton.interactable = true;
        }
        else if(playerManager.isRangedAttack)
        {
            if(playerManager.RangedAttackWeapon == 0)
            {
                swordSelectText.text = "SELECT";
                swordSelectButton.interactable = true;
                StaffSelectText.text = "SELECTED";
                StaffSelectButton.interactable = false;
                BowSelectText.text = "SELECT";
                BowSelectButton.interactable = true;
            }else if(playerManager.RangedAttackWeapon == 1)
            {
                swordSelectText.text = "SELECT";
                swordSelectButton.interactable = true;
                StaffSelectText.text = "SELECT";
                StaffSelectButton.interactable = true;
                BowSelectText.text = "SELECTED";
                BowSelectButton.interactable = false;
            }
        }
        else if(statsModule.weaponType == CharacterStatsModule.WeaponType.RotatingBlades)
        {
            //
        }
    }

    public void SelectMeleeWeapon()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        playerManager.isMeleeAttack = true;
        playerManager.isRangedAttack = false;
        playerManager.isRotatingBlades = false;
        playerManager.PrepareWeapon(0);
        playerManager.updateAttackRangeVisualizer();
        UpdateUI();
    }

    public void SelectStaff()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        playerManager.isMeleeAttack = false;
        playerManager.isRangedAttack = true;
        playerManager.isRotatingBlades = false;
        playerManager.PrepareWeapon(0);
        playerManager.updateAttackRangeVisualizer();
        UpdateUI();
    }

    public void SelectBow()
    {
        GameManager.Instance.soundModule.PlaySound("success");
        playerManager.isMeleeAttack = false;
        playerManager.isRangedAttack = true;
        playerManager.isRotatingBlades = false;
        playerManager.PrepareWeapon(1);
        playerManager.updateAttackRangeVisualizer();
        UpdateUI();
    }
}
