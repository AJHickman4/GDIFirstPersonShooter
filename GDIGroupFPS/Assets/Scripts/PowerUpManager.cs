using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;
    public GameObject iconUnlimitedAmmo;
    public GameObject iconDoubleDamage;

    public bool HasUnlimitedAmmo { get; private set; }
    public bool HasDoubleDamage { get; private set; }
    
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

    public void ActivateUnlimitedAmmo(float duration)
    {
        HasUnlimitedAmmo = true;
        ShowUnlimitedAmmoIcon();
        StartCoroutine(DeactivateUnlimitedAmmoAfterDuration(duration));

    }

    private IEnumerator DeactivateUnlimitedAmmoAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
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
        yield return new WaitForSeconds(duration);
        HasDoubleDamage = false;
        HideDoubleDamageIcon();
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
