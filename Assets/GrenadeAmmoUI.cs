using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeAmmoUI : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> grenadeLogos;

    [SerializeField]
    Shotgun shotgun;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (shotgun.grenadeAmmo == 0)
        {
            grenadeLogos[0].SetActive(false);
            grenadeLogos[1].SetActive(false);
            grenadeLogos[2].SetActive(false);

        }

        if (shotgun.grenadeAmmo == 1)
        {
            grenadeLogos[0].SetActive(true);
            grenadeLogos[1].SetActive(false);
            grenadeLogos[2].SetActive(false);

        }

        if (shotgun.grenadeAmmo == 2)
        {
            grenadeLogos[0].SetActive(true);
            grenadeLogos[1].SetActive(true);
            grenadeLogos[2].SetActive(false);

        }

        if (shotgun.grenadeAmmo == 3)
        {
            grenadeLogos[0].SetActive(true);
            grenadeLogos[1].SetActive(true);
            grenadeLogos[2].SetActive(true);

        }
    }



}
