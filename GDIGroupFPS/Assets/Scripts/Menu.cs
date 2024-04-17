using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    [SerializeField] AudioSource aud;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuMain;
    public void OnPlayClick()
    {
        aud.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OnOptionsClick()
    {
        aud.Play();
        menuOptions.SetActive(true);
        menuMain.SetActive(false);
    }

    public void OnQuitClick()
    {
        aud.Play();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnBackClick()
    {
        aud.Play();
        menuMain.SetActive(true);
        menuOptions.SetActive(false);
    }
}
