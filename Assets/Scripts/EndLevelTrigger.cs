using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelTrigger : MonoBehaviour
{
    public void EndLevel()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Movement.playerMovement.playerInputActions.Player.Disable();
        Movement.playerMovement.playerInputActions.UI.Enable();

        SceneManager.LoadScene(0);
        SoundManager2.Instance.PlayMusic("Main Menu");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) EndLevel();
    }
}
