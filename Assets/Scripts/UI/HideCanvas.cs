using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCanvas : MonoBehaviour
{
    public GameObject canvasToToggle;

    void ToggleOffCanvas()
    {
        canvasToToggle.SetActive(false);
    }
}
