using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Vuforia;

public class Id_Identifier : MonoBehaviour
{
    public SystemControl sis;
    public TextMeshProUGUI nameId;
    public GameObject img;
    public GameObject dem;
    public void Start()
    {
        img.SetActive(false);
    }


    public void Onclick()
    {
        sis.idWorker = nameId.text;
        sis.gameObject.SetActive(true);
        img.SetActive(true);
        gameObject.SetActive(false);
    }
}
