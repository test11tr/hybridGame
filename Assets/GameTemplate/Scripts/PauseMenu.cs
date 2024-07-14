using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public Button pauseButton;
    public Button continueButton;
    public GameObject pausePanel;

    void Start()
    {
        pauseButton.onClick.AddListener(pauseGame);
        continueButton.onClick.AddListener(continueGame);
    }

    public void pauseGame()
    {
        GameManager.Instance.isGamePaused = true;
        Time.timeScale = 0;
        pausePanel.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
    }

    public void continueGame()
    {
        GameManager.Instance.isGamePaused = false;
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }
}
