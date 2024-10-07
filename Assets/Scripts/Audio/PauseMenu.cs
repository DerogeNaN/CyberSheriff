using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //[SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject defaultSelect;
    [SerializeField] private WeaponManagement weaponScript;

    public bool pauseState = false;

    //Called everytime the escape key is pressed in-game
    public void PauseMenuToggle(InputAction.CallbackContext context)
    {
        if (!pauseState)
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
        pauseState = true;

        //Activating player menu input
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Movement.playerMovement.playerInputActions.Player.Disable();
        weaponScript.playerInput.Player.Disable();
        Movement.playerMovement.playerInputActions.UI.Enable();

        //Render pause UI
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming -------------------------------------");
        //Resuming gameplay
        Time.timeScale = 1;
        pauseState = false;

        //Activating player menu input
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Movement.playerMovement.playerInputActions.Player.Enable();
        weaponScript.playerInput.Player.Enable();
        Movement.playerMovement.playerInputActions.UI.Disable();

        //Render pause UI
        pauseMenu.SetActive(false);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
