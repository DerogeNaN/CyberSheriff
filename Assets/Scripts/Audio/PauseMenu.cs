using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //[SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    public bool isPaused = false;

    //Called everytime the escape key is pressed in-game
    public void PauseMenuToggle(InputAction.CallbackContext context)
    {
        if (!isPaused)
        {
            PauseGame();
        }

        else
        {
            ResumeGame();
        }
    }
    
    public void PauseGame()
    {
        Debug.Log("Pausing -------------------------------------"); 
        //Pausing gameplay
        Time.timeScale = 0;
        isPaused = true;

        //Activating player menu input
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Movement.playerMovement.playerInputActions.Player.Disable();
        Movement.playerMovement.playerInputActions.UI.Enable();

        //Render pause UI
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming -------------------------------------");
        //Resuming gameplay
        Time.timeScale = 1;
        isPaused = false;

        //Activating player menu input
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Movement.playerMovement.playerInputActions.Player.Enable();
        Movement.playerMovement.playerInputActions.UI.Disable();

        //Render pause UI
        pauseMenu.SetActive(false);
    }

    public void OptionsButton()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ReturnToMenuButton()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void QuitButton()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(false);

        SceneManager.LoadScene("Alec_Main_Menu");
    }
}
