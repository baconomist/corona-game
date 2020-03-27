using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public GameObject gameUI;
    public TextMeshProUGUI highScore;

    private void Start()
    {
        Time.timeScale = 0;
        if(PlayerPrefs.GetInt("HighScore") <= 0)
            highScore.gameObject.SetActive(false);
    }

    private void Update()
    {
        highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore") + "!";
        
        if (GameManager.Instance.adManager.startupAdShown && !GameManager.Instance.running && (Input.touchCount > 0 || Input.GetMouseButton(0)))
        {
            Time.timeScale = 1;
            GameManager.Instance.running = true;
            
            gameUI.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
