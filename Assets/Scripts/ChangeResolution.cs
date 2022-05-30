using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class ChangeResolution : MonoBehaviour
{
    public Dropdown resolutions;
    public Toggle fullscreen;
    public TextMeshProUGUI directoryText;
    public string directory;

    public void OnEnable()
    {
        
        if (fullscreen != null)
        {
            if (PlayerPrefs.GetInt("ResolutionFullScreen") == 1)
            {
                fullscreen.SetIsOnWithoutNotify(true);
            }
            else
            {
                fullscreen.SetIsOnWithoutNotify(false);
            }
            if (PlayerPrefs.GetInt("ResolutionWidth") == 1280)
            {
                resolutions.value = 0;
            }
            else if (PlayerPrefs.GetInt("ResolutionWidth") == 1366)
            {
                resolutions.value = 1;
            }
            else if (PlayerPrefs.GetInt("ResolutionWidth") == 1920)
            {
                resolutions.value = 2;
            }
        }

        directory = Application.persistentDataPath + "\\" + "CSV";
    }

    public void UpdateResolution()
    {
        bool isFullscreen = true;
        int width = 1366;
        int height = 720;

        switch (resolutions.value)
        {
            case 0:
                width = 1280;
                height = 720;
                break;

            case 1:
                width = 1366;
                height = 768;
                break;

            case 2:
                width = 1920;
                height = 1080;
                break;
        }
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        if (isFullscreen)
        {
            PlayerPrefs.SetInt("ResolutionFullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ResolutionFullscreen", 0);
        }
        
        Screen.SetResolution(width, height, isFullscreen);
    }

    /*
    public void UpdateDirectory()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
        //directory = EditorUtility.OpenFolderPanel("Select Directory", "", "");

    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, "C:/Users", null, "Diretório onde serão salvos os CSVs", "Ok");
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
            }
            directory = FileBrowser.Result[0];
            PlayerPrefs.SetString("Directory", directory);
            directoryText.text = directory;
        }
    }*/


}
