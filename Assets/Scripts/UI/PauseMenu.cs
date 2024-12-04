using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //[SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] public GameObject winScreen;
    [SerializeField] public GameObject loseScreen;
  


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

    public void OnHoverSound()
    {
        SoundManager2.Instance.PlaySound("UIButtonHover");
    }

    public void OnPressSound()
    {
        SoundManager2.Instance.PlaySound("UIButtonPress");
    }

    public void PauseGame()
    {
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

    public void RestartButton()
    {
        Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
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
        SceneManager.LoadScene(0);
        SoundManager2.Instance.PlayMusic("Main Menu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }
}
