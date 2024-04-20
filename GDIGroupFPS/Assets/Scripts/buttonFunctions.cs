using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

    [SerializeField] AudioSource aud;

    public void resume()
    {
        aud.Play();
        gameManager.instance.stateUnPaused();
    }

    public void restart()
    {
        
        aud.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnPaused();
    }
    
    public void respawn()
    {
        aud.Play();
        gameManager.instance.stateUnPaused();
        gameManager.instance.playerScript.spawnPlayer();
    }

    public void quit()
    {
        aud.Play();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
