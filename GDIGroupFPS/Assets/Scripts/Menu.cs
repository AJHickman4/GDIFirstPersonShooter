using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuExitButton;

    [SerializeField] GameObject sensitivityText;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] GameObject sensitivityValue;

    private void Awake()
    {
        if (menuOptions.activeSelf == false)
            menuOptions.SetActive(true);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (menuExitButton)
            {
                menuExitButton.SetActive(false);
            }
        }

        //if (menuMain)
        //{
        //    if (sensitivityText && sensitivitySlider)
        //    {
        //        //sensitivitySlider.enabled = false;
        //    }
        //}
    }
    public void OnPlayClick()
    {
        aud.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OnOptionsClick()
    {
        aud.Play();
        menuOptions.SetActive(true);
        if (menuMain)
            menuMain.SetActive(false);
        else if (menuPause)
            menuPause.SetActive(false);
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

    public void OnCreditsClick()
    {
        aud.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void OnBackClick()
    {
        aud.Play();
        menuOptions.SetActive(false);
        if (menuMain)
            menuMain.SetActive(true);
        else if (menuPause)
            menuPause.SetActive(true);
    }
}
