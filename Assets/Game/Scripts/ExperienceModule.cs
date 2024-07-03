using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExperienceModule : MonoBehaviour
{
    [Foldout("Experience - References", foldEverything = true, styled = true, readOnly = false)]
    public TMP_Text experienceText;
    public TMP_Text levelText;
    public Image experienceBar;

    [Foldout("Experience - Values (Debug Puroses)", foldEverything = true, styled = true, readOnly = true)]
    public int _currentExperience;
    public int _maxExperience;
    public int _currentlevel;

    public delegate void ExperienceChangeHandler(int experience);
    public event ExperienceChangeHandler OnExperienceChange;
    
    public void AddExperience(int experience)
    {
        OnExperienceChange?.Invoke(experience);
    }

    public void UpdateExperienceUI(int currentExperience, int currentlevel, int maxExperience)
    {
        _currentExperience = currentExperience;
        _maxExperience = maxExperience;
        _currentlevel = currentlevel;
        
        string formattedCurrentExperience = NumberFormatter.Convert(currentExperience);
        string formattedMaxExperience = NumberFormatter.Convert(maxExperience);
        
        experienceText.text = $"{formattedCurrentExperience} / {formattedMaxExperience}";
        levelText.text = (currentlevel+1).ToString();
        experienceBar.fillAmount = (float)currentExperience / maxExperience;
    }    
}
