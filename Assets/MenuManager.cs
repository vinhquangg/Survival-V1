using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }

    public GameObject menuCanvas;
    public GameObject uiCanvas;
    PlayerController PlayerController;
    public bool isMenuOpen;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PlayerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&& !isMenuOpen)
        {
            uiCanvas.SetActive(false);
            menuCanvas.SetActive(true);
            isMenuOpen= true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerController.inputHandler.DisablePlayerInput();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isMenuOpen)
        {
            uiCanvas.SetActive(true);
            menuCanvas.SetActive(false);
            isMenuOpen = false;
            PlayerController.inputHandler.EnablePlayerInput();
        }
    }
}
