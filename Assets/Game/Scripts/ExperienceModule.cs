using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceModule : MonoBehaviour
{
    [Header("Experience - References")]
    public TMP_Text experienceText;
    public TMP_Text levelText;
    public Image experienceBar;
    
    [Header("Experience - Values (Debug Puroses)")]
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
        
        experienceText.text = currentExperience.ToString() + " / " + maxExperience.ToString();
        levelText.text = (currentlevel+1).ToString();
        experienceBar.fillAmount = (float)currentExperience / maxExperience;
    }    
}
