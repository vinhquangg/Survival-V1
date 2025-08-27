using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        GameManager.instance.SetState(GameState.MainMenu);
        GameManager.instance.SetCursorLock(false);
        SceneManager.LoadScene("MainMenu");
    }

}
