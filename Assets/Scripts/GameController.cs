/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using Cheers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{	
    [ClickMe]
	public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AlchemyLibrary.Instance.Reload();
        AlchemyLab.Instance.Reset();
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    [SerializeField] GameObject _menuGo;
    [SerializeField] GameObject _tutorialGo;

    private void Start()
    {
        if (_menuGo != null) _menuGo.SetActive(true);
        if (_tutorialGo != null) _tutorialGo.SetActive(false);
    }

    public void ToTutorial()
    {
        if(_menuGo != null) _menuGo.SetActive(false);
        if(_tutorialGo != null) _tutorialGo.SetActive(true);
    }
}
