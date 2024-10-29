using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_Sound : MonoBehaviour
{
    

    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            //SoundManager.Instance.PlaySound("Footsteps_Concrete");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SoundManager2.Instance.PlaySound("Footsteps_Concrete");
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SoundManager2.Instance.PlayMusic("2");
        }
    }


}
