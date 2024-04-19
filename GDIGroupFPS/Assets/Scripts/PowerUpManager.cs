using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;
    public GameObject iconUnlimitedAmmo;
    public GameObject iconDoubleDamage;
    public playerController player;
    public ParticleSystem ultimateModeParticles;
    private Color originalParticleColor;
    public FlyingDrone droneController;

    public bool HasUnlimitedAmmo { get; private set; }
    public bool HasDoubleDamage { get; private set; }
    public bool HasForceField { get; private set; }

    public float ultimateModeDuration = 5f;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<playerController>(); 
        }
        else
        {
            //Debug
        }
        if (ultimateModeParticles != null)
        {
            var main = ultimateModeParticles.main;
            originalParticleColor = main.startColor.color; 
        }
        else
        {
            Debug.LogError("Ultimate mode particle system not set.");
        }
    }
    void Update()
    {
        if (HasUnlimitedAmmo && HasDoubleDamage && HasForceField)
        {
            ActivateUltimateMode();
        }
    }
    private void ActivateUltimateMode()
    {
        Debug.Log("Ultimate Mode Activated!");
        StartCoroutine(UltimateModeEffects());
    }
    private IEnumerator UltimateModeEffects()
    {
        float startTime = Time.realtimeSinceStartup;
        if (droneController != null)
        {
            //droneController.ToggleActivation(); Idk yet
        }
        Color redWithOriginalAlpha = new Color(1, 0, 0, 0.25f);
        ChangeParticleSystemColor(redWithOriginalAlpha);
        while (Time.realtimeSinceStartup - startTime < ultimateModeDuration)
        {
            yield return null; 
        }
        if (droneController != null)
        {
            //droneController.ToggleDeactivation(); Idk yet
        }
        ChangeParticleSystemColor(originalParticleColor);
        ResetAllPowerUps();
    }
   
    public void ActivateUnlimitedAmmo(float duration)
    {
        HasUnlimitedAmmo = true;
        ShowUnlimitedAmmoIcon();
        StartCoroutine(DeactivateUnlimitedAmmoAfterDuration(duration));

    }
   
    private IEnumerator DeactivateUnlimitedAmmoAfterDuration(float duration)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < duration)
        {
            yield return null;
        }
        HideUnlimitedAmmoIcon();
        HasUnlimitedAmmo = false;
    }
    public void ActivateDoubleDamage(float duration)
    {
        HasDoubleDamage = true;
        ShowDoubleDamageIcon();
        StartCoroutine(DeactivateDoubleDamageAfterDuration(duration));
    }
    private IEnumerator DeactivateDoubleDamageAfterDuration(float duration)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < duration)
        {
            yield return null;
        }
        HasDoubleDamage = false;
        HideDoubleDamageIcon();
    }
    private void ResetAllPowerUps()
    {
        HasUnlimitedAmmo = false;
        HasDoubleDamage = false;
        HasForceField = false;
    }
    public void SetForceFieldActive(bool isActive)
    {
        HasForceField = isActive;
    }
    void ChangeParticleSystemColor(Color newColor)
    {
        if (ultimateModeParticles != null)
        {
            var main = ultimateModeParticles.main;
            Color currentColor = main.startColor.color;
            newColor.a = currentColor.a;
            main.startColor = newColor;
        }
        else
        {
            //Debug
        }
    }
    void ShowUnlimitedAmmoIcon()
    {
        if (iconUnlimitedAmmo != null) iconUnlimitedAmmo.SetActive(true);
    }

    void HideUnlimitedAmmoIcon()
    {
        if (iconUnlimitedAmmo != null) iconUnlimitedAmmo.SetActive(false);
    }

    void ShowDoubleDamageIcon()
    {
        if (iconDoubleDamage != null) iconDoubleDamage.SetActive(true);
    }

    void HideDoubleDamageIcon()
    {
        if (iconDoubleDamage != null) iconDoubleDamage.SetActive(false);
    }
}
