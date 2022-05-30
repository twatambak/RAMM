using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step_Controller : MonoBehaviour
{

    public GameObject[] steps;
    public GameObject[] partsDemonstration;
    public GameObject menuSteps;
    public GameObject rastrPos;

    private void Start()
    {
        foreach(GameObject a in steps)
        {
            a.SetActive(false);
        }
        foreach (GameObject a in partsDemonstration)
        {
            a.SetActive(false);
        }
        rastrPos.SetActive(false);
    }

    public void OnClick(int index)
    {
        steps[index].SetActive(true);
        partsDemonstration[index].SetActive(true);
        menuSteps.SetActive(false);
        rastrPos.SetActive(true);
    }
}
