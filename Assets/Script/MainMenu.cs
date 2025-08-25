
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour,IMenu
{
    public void NewGame()
    {
        SceneManager.LoadScene("GameScene");
        GameManager.instance.SetState(GameState.Playing);
        GameManager.instance.SetCursorLock(true);   
    }

    public void OpenMenu()
    {
        GameManager.instance.SetState(GameState.MainMenu);
        GameManager.instance.SetCursorLock(false);
    }

    public void CloseMenu()
    {
       
    }
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }


}
