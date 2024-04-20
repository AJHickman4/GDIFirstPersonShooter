using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
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
    SpriteRenderer damageIndi;
    public Image healthBar;
    public Image damageBar;
    public Image staminaBar;
    public GameObject boardActive;

    public float resetTimer = 30f;
    private float currentTime;
    public bool isResetting = false;

    public GameObject exitDoorPrompt;
    [SerializeField] TMP_Text keysNeededText;

    public GameObject player;
    public playerController playerScript;

    public ShopManager shopManager; //ref to new shop manager. needed for pause menu

    public Weapon currentWeapon;

    public GameObject startingSpawn;
    public bool isPaused;
    float timeScaleOrig;
    int enemyCount;
    public bool timerIsActive = false;
    private bool isFlashing = false;
    bool temp;
    public ParticleSystem teleportEffect;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timeScaleOrig = Time.timeScale;
        startingSpawn = GameObject.FindWithTag("Starting Spawnpoint");
        currentTime = resetTimer;
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            UpdateTimerUI(currentTime);
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
            gameManager.instance.UpdateAmmoUI(currentWeapon.currentAmmo, currentWeapon.totalAmmoReserve);
        }
        if (menuActive == startingDialog && Input.anyKey)
        {


            stateUnPaused();


        }
        if (inputManager.instance.MenuOpenCloseInput)
        {
            if(menuActive == null)
            {
                statePaused();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else
            {
                stateUnPaused();
            }
        }
        
        if (timerIsActive)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI(currentTime);
                if (currentTime <= 4f && currentTime > 3f)
                {
                    if (!isResetting)
                    {
                        PlayTeleportEffect();
                        isResetting = true;
                    }
                }
            }
            else
            {
                if (isResetting)
                {
                    StartCoroutine(TeleportPlayerToSpawn());
                    currentTime = 0;
                    UpdateTimerUI(currentTime);
                    timerIsActive = false;
                    isResetting = false;
                }
            }
        }
        //if (Input.GetKeyDown(KeyCode.P))
        //    {
        //        if(isPaused)
        //        {
        //            stateUnPaused(); //resume the game 
        //        }
        //        else
        //        {
        //            statePaused(); //pauses game if not paused
        //            shopManager.gameObject.SetActive(true); //activate shop
        //        }
        //    }
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

        {
            isResetting = true;
            playerScript.controller.enabled = false;
            yield return new WaitForSeconds(0.5f);
            player.transform.position = startingSpawn.transform.position;
            playerScript.controller.enabled = true;
            playerScript.HP = playerScript.HPOrig;
            playerScript.updatePlayerUI();

            currentTime = resetTimer;
            UpdateTimerUI(currentTime);
            timerIsActive = false;
            teleportEffect.Stop();
            yield return new WaitForSeconds(2f);
            teleportEffect.Clear();

            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
            }
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
    public void CancelAndResetTimer()
    {
        if (timerIsActive)
        {
            StopCoroutine("ResetTimerCoroutine");
        }
        timerIsActive = false;
        currentTime = resetTimer;

        if (timerText != null)
        {
            UpdateTimerUI(currentTime);
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

    public void IncreaseResetTimer(float additionalTime)
    {
        resetTimer += additionalTime;
        currentTime += additionalTime;
        UpdateTimerUI(currentTime);
    }

    void PlayTeleportEffect()
    {
        if (teleportEffect != null)
        {
            teleportEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            teleportEffect.Play();
        }
    }

    //public void Damaged()
    //{
    //    StartCoroutine(FadeIn());
    //    StartCoroutine(FadeOut());

    //}


}

