using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Menu's")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject leaderboardMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;
    //[SerializeField] GameObject iutMenu;
    //[SerializeField] GameObject gameMenu;
    //[SerializeField] GameObject graphicsMenu;
    //[SerializeField] GameObject MiscMenu;

    

    private GameObject currentMenu = null;
    private GameObject previousMenu = null;

    private void Awake()
    {
        Time.timeScale = 1;
        ReturnToMenuButton();
    }

    public void ReturnToMenuButton()
    {
        mainMenu.SetActive(true);
        leaderboardMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        previousMenu = currentMenu;
        currentMenu = mainMenu;
    }

    public void PlayButton()
    {
        mainMenu.SetActive(false);
        leaderboardMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        SceneManager.LoadScene(1);
    }

    public void LeaderboardButton()
    {
        mainMenu.SetActive(false);
        leaderboardMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        previousMenu = currentMenu;
        currentMenu = leaderboardMenu;
    }

    public void OptionsButton()
    {
        mainMenu.SetActive(false);
        leaderboardMenu.SetActive(false);
        optionsMenu.SetActive(true);
        creditsMenu.SetActive(false);

        previousMenu = currentMenu;
        currentMenu = optionsMenu;
    }

    public void CreditsButton()
    {
        mainMenu.SetActive(false);
        leaderboardMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(true);

        previousMenu = currentMenu;
        currentMenu = creditsMenu;
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}