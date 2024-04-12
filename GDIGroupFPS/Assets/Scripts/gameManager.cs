using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuKey1;
    [SerializeField] GameObject menuKey2;
    [SerializeField] GameObject menuKey3;
    public GameObject menuCheckpoint;
    [SerializeField] TMP_Text CreditsText;
    [SerializeField] TMP_Text bulletCountText;
    [SerializeField] GameObject startingDialog;
    [SerializeField] GameObject iconDoubleDamage;
    [SerializeField] GameObject iconShield;
    [SerializeField] GameObject iconUnlimtedAmmo;
    [SerializeField] TMP_Text timerText;
    [SerializeField] private float flashThreshold = 10f;
    [SerializeField] private float flashDuration = 0.5f;

    public GameObject damageIndicator;
    public Image healthBar;
    public Image staminaBar;
    public GameObject boardActive;

    public float resetTimer = 30f;
    private float currentTime;
    public bool isResetting = false;

    public GameObject exitDoorPrompt;
    [SerializeField] TMP_Text keysNeededText;

    public GameObject player;
    public playerController playerScript;

    public Weapon currentWeapon;

    public GameObject startingSpawn;
    public bool isPaused;
    float timeScaleOrig;
    int enemyCount;
    public bool timerIsActive = false;
    private bool isFlashing = false;
    bool temp;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timeScaleOrig = Time.timeScale;
        startingSpawn = GameObject.FindWithTag("Starting Spawnpoint");
        currentTime = resetTimer;

        OnShieldActivated += ShowShieldIcon;
        OnShieldDeactivated += HideShieldIcon;
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
        // This code pauses the game and starts the beginning dialogue screen
        statePaused();
        menuActive = startingDialog;
        menuActive.SetActive(isPaused);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeapon != null)
        {
            gameManager.instance.UpdateAmmoUI(currentWeapon.currentAmmo,currentWeapon.totalAmmoReserve);
        }
        if (menuActive == startingDialog && Input.anyKey)
        {
            stateUnPaused();
        }
        if (Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePaused();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
        if (timerIsActive)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI(currentTime);
            }
            else
            {
                StartCoroutine(TeleportPlayerToSpawn());
                currentTime = 0;
                UpdateTimerUI(currentTime);
                timerIsActive = false;
            }
        }
    }

    public static event Action OnShieldActivated;
    public static event Action OnShieldDeactivated;

    void OnDestroy()
    { 
        OnShieldActivated -= ShowShieldIcon;
        OnShieldDeactivated -= HideShieldIcon;
    }

    public void ShowShieldIcon() => iconShield.SetActive(true);
    public void HideShieldIcon() => iconShield.SetActive(false);


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
        for (int i = 0; i < playerScript.keys.Count; i++)
        {
            if (playerScript.keys[i] == 1)
                menuKey1.SetActive(true);
            if (playerScript.keys[i] == 2)
                menuKey2.SetActive(true);
            if (playerScript.keys[i] == 3)
                menuKey3.SetActive(true);
        }
    }

    public void UpdateAmmoUI(int currentAmmo, int totalAmmoReserve)
    {
        bulletCountText.text = $"{totalAmmoReserve} / {currentAmmo}";
    }


public void updateCreditsUI()
    {
        CreditsText.text = playerScript.credits.ToString("F0");
    }

    public void updateKeysNeededUI(int keysPlayerNeeds)
    {
        keysNeededText.text = keysPlayerNeeds.ToString("F0");
    }

    IEnumerator ResetTimerCoroutine()
    {
        yield return new WaitForSeconds(resetTimer);
        StartCoroutine(TeleportPlayerToSpawn());
    }

    IEnumerator TeleportPlayerToSpawn()
    {
        if (!isResetting)
        {
            isResetting = true;
            playerScript.controller.enabled = false;
            player.transform.position = startingSpawn.transform.position;
            playerScript.controller.enabled = true;
            playerScript.HP = playerScript.HPOrig;
            playerScript.updatePlayerUI();
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false);
            }
            timerIsActive = false;
            currentTime = 0;
            isResetting = false;
        }
        yield return null; 
    }
   
    public void StartResetTimer()
    {
        if (!timerIsActive)
        {
            currentTime = resetTimer;
            timerIsActive = true;
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true); 
            }
            StartCoroutine(ResetTimerCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) 
        {
            StartResetTimer();
        }
    }
    private void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            timerText.text = "Reset in: " + time.ToString("F2") + "s";
            if (time <= flashThreshold && !isFlashing)
            {
                StartCoroutine(FlashTimerUI());
            }
            else if (time > flashThreshold && isFlashing)
            {
                StopCoroutine(FlashTimerUI());
                isFlashing = false;
                timerText.color = Color.white;
            }
        }
    }

    IEnumerator FlashTimerUI()
    {
        isFlashing = true;
        bool isRed = false;
        while (isFlashing)
        {
            timerText.color = isRed ? Color.white : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(flashDuration);
        }
        timerText.color = Color.white;
    }



}

