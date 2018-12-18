﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();

        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    public void RestartLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
