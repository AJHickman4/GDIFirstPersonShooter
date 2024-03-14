using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuKey;
    [SerializeField] TMP_Text enemyCountText;
    public GameObject damageIndicator;
    public Image healthBar;

    public GameObject player;
    public playerController playerScript;

    public bool isPaused;
    float timeScaleOrig;
    int enemyCount;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
    }

    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnPaused()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }
    public void youHaveWon()
    {
        statePaused(); // on trigger enter in player script, if player touches tagged win collider then gameManager.instance.youHaveWon
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
    public void youHaveLost()
    {
        statePaused(); // on trigger enter in player script, if player touches tagged win collider then gameManager.instance.youHaveWon
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void updateKeyUI()
    {
            menuKey.SetActive(true);
    }
}