using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //[SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject firstPMBtn;
    [SerializeField] private GameObject settingsMenu;


    public Scene scene;

    public bool pauseState = false;
    public bool settingsMenuState = false;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        //if audio menu is open, closes audio_menu
        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenuState == true)
        {
            CloseAudioMenu();
            return;
        }

        //opens pause_menu
        if (Input.GetKeyDown(KeyCode.Escape) && (pauseState == false) && (settingsMenuState == false))
        {
            OpenPauseMenu();
            return;
        }

        //if audio menu is not open, closes pause_menu 
        if (Input.GetKeyDown(KeyCode.Escape) && pauseState == true)
        {
            Return();
        }
    }

    public void PauseMenuToggle()
    {
        //if audio menu is open, closes audio_menu
        if (settingsMenuState == true)
        {
            CloseAudioMenu();
            return;
        }

        //opens pause_menu
        if (settingsMenuState == false)
        {
            //playerManager.JournalToggle();

            OpenPauseMenu();
            return;
        }

        //if audio menu is not open, closes pause_menu 
        if (pauseState == true)
        {
            Return();
        }
    }

    //Return
    public void Return()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        pauseState = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // Hides the cursor and locks it to the center of the screen
    }


    //Opens pause_menu
    private void OpenPauseMenu()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        pauseState = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstPMBtn);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Shows the cursor and unlocks it
    }

    //options button to open options
    public void Options()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        settingsMenuState = true;
        pauseState = false;
    }

    //closes audio menu and returns to pause_menu
    public void CloseAudioMenu()
    {
        OpenPauseMenu();
        settingsMenu.SetActive(false);
        settingsMenuState = false;
        return;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
