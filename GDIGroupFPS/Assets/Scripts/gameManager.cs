using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] TMP_Text magCountText;
    [SerializeField] GameObject startingDialog;
    [SerializeField] GameObject iconDoubleDamage;
    [SerializeField] GameObject iconShield;
    [SerializeField] GameObject iconUnlimtedAmmo;
    [SerializeField] TMP_Text timerText;

    public GameObject damageIndicator;
    public Image healthBar;
    public GameObject boardActive;

    public float resetTimer = 30f;
    private float currentTime;

    public GameObject exitDoorPrompt;
    [SerializeField] TMP_Text keysNeededText;

    public GameObject player;
    public playerController playerScript;

    public Weapon currentWeapon;

    public GameObject startingSpawn;
    public bool isPaused;
    float timeScaleOrig;
    int enemyCount;
    private bool timerIsActive = false;

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
        currentTime = 0;
        Weapon.OnDoubleDamageActivated += ShowDoubleDamageIcon;
        Weapon.OnDoubleDamageDeactivated += HideDoubleDamageIcon;
        Weapon.OnUnlimitedAmmoActivated += ShowUnlimitedAmmoIcon;
        Weapon.OnUnlimitedAmmoDeactivated += HideUnlimitedAmmoIcon;
        Weapon.OnShieldActivated += ShowShieldIcon;
        Weapon.OnShieldDeactivated += HideShieldIcon;

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
            UpdateAmmoUI(currentWeapon.currentAmmo, currentWeapon.currentMags, currentWeapon.ammoPerMag, currentWeapon.totalMags);
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
                currentTime = 0;
                UpdateTimerUI(currentTime);
                timerIsActive = false;
                TeleportPlayerToSpawn();
            }
        }
    }
    void OnDestroy()
    {
        Weapon.OnDoubleDamageActivated -= ShowDoubleDamageIcon;
        Weapon.OnDoubleDamageDeactivated -= HideDoubleDamageIcon;
        Weapon.OnUnlimitedAmmoActivated -= ShowUnlimitedAmmoIcon;
        Weapon.OnUnlimitedAmmoDeactivated -= HideUnlimitedAmmoIcon;
        Weapon.OnShieldActivated -= ShowShieldIcon;
        Weapon.OnShieldDeactivated -= HideShieldIcon;
    }

    void ShowDoubleDamageIcon() => iconDoubleDamage.SetActive(true);
    void HideDoubleDamageIcon() => iconDoubleDamage.SetActive(false);
    void ShowUnlimitedAmmoIcon() => iconUnlimtedAmmo.SetActive(true);
    void HideUnlimitedAmmoIcon() => iconUnlimtedAmmo.SetActive(false);
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

    public void UpdateAmmoUI(int currentBullets, int currentMags, int maxBulletsPerMag, int maxMags)
    {
        bulletCountText.text = $"{currentBullets} / {maxBulletsPerMag}";
        magCountText.text = $"{currentMags} / {maxMags}";
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
        TeleportPlayerToSpawn();
        timerIsActive = false; 
        currentTime = 0; 
    }
    
    public void TeleportPlayerToSpawn()
    {
        if (player != null && startingSpawn != null)
        {
            player.transform.position = startingSpawn.transform.position;
            playerScript.HP = playerScript.HPOrig;
            playerScript.updatePlayerUI();
        }
    }
    public void StartResetTimer()
    {
        if (!timerIsActive) 
        {
            currentTime = resetTimer; 
            timerIsActive = true; 
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
        }
    }
}

