using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;
    public GameObject iconUnlimitedAmmo;
    public GameObject iconDoubleDamage;
    public playerController player;
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
    }

    public void ActivateUnlimitedAmmo(float duration)
    {
        HasUnlimitedAmmo = true;
        ShowUnlimitedAmmoIcon();
        StartCoroutine(DeactivateUnlimitedAmmoAfterDuration(duration));

    }

    private IEnumerator DeactivateUnlimitedAmmoAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration * 0.8f); 
        StartCoroutine(FlashIconColor(iconUnlimitedAmmo, duration * 0.2f));
        yield return new WaitForSeconds(duration * 0.2f); 
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
        yield return new WaitForSeconds(duration * 0.8f); 
        StartCoroutine(FlashIconColor(iconDoubleDamage, duration * 0.2f));
        yield return new WaitForSeconds(duration * 0.2f); 
        HideDoubleDamageIcon();
        HasDoubleDamage = false;
    }

    public void SetForceFieldActive(bool isActive)
    {
        HasForceField = isActive;
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
    IEnumerator FlashIconColor(GameObject icon, float duration)
    {
        if (icon == null)
            yield break;
        var imageComponent = icon.GetComponent<UnityEngine.UI.Image>();
        if (imageComponent == null)
        {
            yield break;
        }
        float endTime = Time.time + duration;
        bool isRed = false;
        while (Time.time < endTime)
        {
            imageComponent.color = isRed ? Color.white : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(0.2f);
        }
        imageComponent.color = Color.white;
    }
}
