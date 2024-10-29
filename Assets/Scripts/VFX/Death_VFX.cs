using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Death_VFX : MonoBehaviour
{
    public Animator anim;
    public VisualEffect Death_Particles;

    private bool dying;

    void Update()
    {
        if(anim != null)
        {
            if (Input.GetButtonDown("Fire1") && !dying)
            {
                anim.SetTrigger("Die");

                if (Death_Particles != null)
                    Death_Particles.Play();

                dying = true;
                StartCoroutine(ResetBool(dying, 0.5f));
            }
        }
    }

    IEnumerator ResetBool (bool boolToReset, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        dying = !dying;
    }
}
