using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public string scene;
    public SystemControl sis;
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            sis.StopStepWritter();
            SceneManager.LoadScene(scene);
        }
    }

    public void Load()
    {
        SceneManager.LoadScene(scene);
    }
}
