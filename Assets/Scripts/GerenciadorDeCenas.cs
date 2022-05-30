using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GerenciadorDeCenas : MonoBehaviour
{

    public void LoadScene(string name)
    {

        SceneManager.LoadScene(name);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void ActivateGameObject(GameObject objectToActive)
    {
        objectToActive.SetActive(!objectToActive.activeSelf);
    }
}
