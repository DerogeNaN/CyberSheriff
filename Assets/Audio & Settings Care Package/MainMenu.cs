using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class MainMenu : MonoBehaviour
{
   // [SerializeField] private GameObject titleMenu;
    //[SerializeField] private GameObject titleMenuFirstButton;

    [SerializeField] private GameObject mainMenu;
    //[SerializeField] private GameObject mainMenuFirstButton;

    [SerializeField] private GameObject settingsMenu;
    //[SerializeField] private GameObject settingsFirstButton;

    [SerializeField] private GameObject creditsMenu;
    //[SerializeField] private GameObject creditsFirstButton;

    [SerializeField] private GameObject controlsMenu;
    //[SerializeField] private GameObject controlsFirstButton;

    // [SerializeField] private GameObject playMenu;
    // [SerializeField] private GameObject playFirstButton;

    // [SerializeField] private GameObject localPlayMenu;
    // [SerializeField] private GameObject localPlayFirstButton;

    //[SerializeField] private GameObject lobbyMenu;
    //[SerializeField] private GameObject lobbyFirstButton;

    // [SerializeField] private GameObject lobbyControlsMenu;
    //[SerializeField] private GameObject lobbyControlsFirstButton;

    // [SerializeField] private GameObject trackSelectMenu;
    //[SerializeField] private GameObject trackSelectFirstButton;

  // [SerializeField] private GameObject pauseMenu;
    //[SerializeField] private GameObject hudOverlay;

    //[SerializeField] private GameObject menuScene;
    // [SerializeField] private GameObject demoScene;

   /* public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        hudOverlay.SetActive(false);
        
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(settingsFirstButton);

    }
    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        hudOverlay.SetActive(true);

    }*/
    /*public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        titleMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(settingsFirstButton);

    }*/

    //Main Screen Buttons

    /* public void OpenPlayMenu()
     {
         playMenu.SetActive(true);
         mainMenu.SetActive(false);
         //EventSystem.current.SetSelectedGameObject(null);
         //EventSystem.current.SetSelectedGameObject(settingsFirstButton);

     }

     public void ClosePlayMenu()
     {
         playMenu.SetActive(false);
         mainMenu.SetActive(true);
         //EventSystem.current.SetSelectedGameObject(null);
         //EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);

         return;
     }*/

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(settingsFirstButton);

    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);

        return;
    }
    public void OpenCreditsMenu()
    {
        creditsMenu.SetActive(true);
        mainMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(creditsFirstButton);
    }

    public void CloseCreditsMenu()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
       //EventSystem.current.SetSelectedGameObject(null);
       //EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
        return;
    }
    public void QuitGame()
    {
        Application.Quit();

    }

    //Play Menu

   /* public void OpenLocalPlayMenu()
    {
        localPlayMenu.SetActive(true);
        playMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(settingsFirstButton);

    }*/

    /*public void CloseLocalPlayMenu()
    {
        localPlayMenu.SetActive(false);
        playMenu.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);

        return;
    }*/

    //Settings Button

    public void OpenControlsMenu()
    {
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(controlsFirstButton);
        
    }

    public void CloseControlsMenu()
    {
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(settingsFirstButton);
        
    }



    /*public void OpenLobbyPlayMenu()
    {
        lobbyMenu.SetActive(true);
        localPlayMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(settingsFirstButton);

    }*/

   /* public void CloseLobbyPlayMenu()
    {
        lobbyMenu.SetActive(false);
        localPlayMenu.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);

        return;
    }*/

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void StartLevelArcade()
    {
        Invoke("Delay", 5);
    }

   /* public void Delay()
    {
        menuScene.SetActive(false);
        demoScene.SetActive(true);
    }*/

    public void Awake()
    {
        Invoke("LatePlay", 1);
    }

    void LatePlay()
    {
        //AudioManager.instance.PlaySound("MainMenu");
    }
}
