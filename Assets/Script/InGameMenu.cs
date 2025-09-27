using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject inGameMenuUI;
    public GameObject hubUI;
    public void BackToMainMenu()
    {
        GameManager.instance.SetState(GameState.MainMenu);
        GameManager.instance.SetCursorLock(false);
        SceneManager.LoadScene("MainMenu");
    }

    //public void ResumeGame()
    //{
    //    GameManager.instance.SetState(GameState.Playing);
    //    Time.timeScale = 1f;
    //    Cursor.visible = false;
    //    if (inGameMenuUI != null)
    //        inGameMenuUI.SetActive(false);
    //    if (hubUI != null)
    //        hubUI.SetActive(true);
    //    GameManager.instance.SetCursorLock(true);
    //}

}
