using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipScript : MonoBehaviour
{
    [Header("Drag in")]
    public Transform PlayerTransform;
    public Camera playerCamera;
    public playerController PlayerController;

    [Header("Range")]
    [Range(1, 5)] public float equipRange = 5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    [Header("Drop")]
    public KeyCode dropKey = KeyCode.F;

    private List<GameObject> guns = new List<GameObject>();
    private int equippedGunIndex = -1;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            TryEquipObjectWithRaycast();
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            int newIndex = equippedGunIndex + (scroll > 0f ? 1 : -1);
            if (newIndex < 0)
            {
                newIndex = guns.Count - 1;
            }
            else if (newIndex >= guns.Count)
            {
                newIndex = 0;
            }
            SwitchGun(newIndex);
        }
    }

    void TryEquipObjectWithRaycast()
    {
        float sphereRadius = 1f;
        float maxDistance = equipRange;
        
        RaycastHit hit;
        if (Physics.SphereCast(playerCamera.transform.position, sphereRadius, playerCamera.transform.forward, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Gun"))
            {
                GameObject gun = hit.collider.gameObject;
                if (!guns.Contains(gun))
                {
                    if (guns.Count < 2)
                    {
                        EquipObject(gun);
                        
                    }
                    else
                    {
                        DestroyCurrentGun();
                        EquipObject(gun);
                    }
                }
            }
        }
    }

    void EquipObject(GameObject gun)
    {
        if (equippedGunIndex >= 0 && equippedGunIndex < guns.Count)
        {
            GameObject currentlyEquippedGun = guns[equippedGunIndex];
            SetActiveGunComponents(currentlyEquippedGun, false);
            Weapon currentWeaponScript = currentlyEquippedGun.GetComponent<Weapon>();
            if (currentWeaponScript != null)
            {
                currentWeaponScript.SetEquipped(false);
            }
        }
        guns.Add(gun);
        gun.GetComponent<Rigidbody>().isKinematic = true;
        gun.transform.position = PlayerTransform.position;
        gun.transform.rotation = PlayerTransform.rotation;
        gun.transform.SetParent(PlayerTransform);
        gun.layer = LayerMask.NameToLayer("weapons");

        equippedGunIndex = guns.Count - 1;

        SetActiveGunComponents(gun, true); 

        Weapon weaponScript = gun.GetComponent<Weapon>();
        if (weaponScript != null)
        {
            weaponScript.SetEquipped(true);
            gameManager.instance.UpdateAmmoUI(weaponScript.currentAmmo, weaponScript.totalAmmoReserve);
        }

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }


    IEnumerator SwitchGunWithDelay(int index)
    {
        yield return new WaitForSeconds(0f); 
        if (index >= 0 && index < guns.Count && index != equippedGunIndex && PlayerController.isMeleeReady)
        {
            Weapon weaponScript = guns[equippedGunIndex].GetComponent<Weapon>();
            if (weaponScript != null && !weaponScript.isReloading && weaponScript.readyToShoot && !weaponScript.isRecoiling)
            {
                SetActiveGunComponents(guns[equippedGunIndex], false);  
                equippedGunIndex = index;
                SetActiveGunComponents(guns[equippedGunIndex], true);  
                weaponScript = guns[equippedGunIndex].GetComponent<Weapon>();
                if (weaponScript != null)
                {
                    gameManager.instance.UpdateAmmoUI(weaponScript.currentAmmo, weaponScript.totalAmmoReserve);
                }
                for (int i = 0; i < guns.Count; i++)
                {
                    Weapon script = guns[i].GetComponent<Weapon>();
                    if (script != null)
                    {
                        script.isEquipped = (i == equippedGunIndex);
                    }
                }
            }
        }
    }
    void SetActiveGunComponents(GameObject gun, bool active)
    {
        MeshRenderer renderer = gun.GetComponent<MeshRenderer>();
        Collider collider = gun.GetComponent<Collider>();

        if (renderer != null)
        {
            renderer.enabled = active;
        }
        if (collider != null)
        {
            collider.enabled = active;
        }
        foreach (var otherGun in guns)
        {
            if (otherGun != gun)
            {
                MeshRenderer otherRenderer = otherGun.GetComponent<MeshRenderer>();
                Collider otherCollider = otherGun.GetComponent<Collider>();
                if (otherRenderer != null)
                {
                    otherRenderer.enabled = !active;
                }
                if (otherCollider != null)
                {
                    otherCollider.enabled = !active;
                }

                Weapon otherWeapon = otherGun.GetComponent<Weapon>();
                if (otherWeapon != null)
                {
                    otherWeapon.isEquipped = false;
                }
            }
        }
    }

    public void DeactivateAllGuns()
    {
        foreach (var gun in guns)
        {
            gun.SetActive(false); 
            Weapon weapon = gun.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapon.isEquipped = false; 
            }
        }
    }

    public void ReactivateAllGuns()
    {
        foreach (var gun in guns)
        {
            gun.SetActive(true); 
            Weapon weapon = gun.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapon.isEquipped = (gun == guns[equippedGunIndex]);
            }
        }
    }

    void SetActiveGun(GameObject gun, bool active)
    {
        if (gun != null)
        {
            gun.SetActive(active);
            foreach (var otherGun in guns)
            {
                if (otherGun != gun)
                {
                    otherGun.SetActive(!active);
                    Weapon otherWeapon = otherGun.GetComponent<Weapon>();
                    if (otherWeapon != null)
                    {
                        otherWeapon.isEquipped = active;
                    }
                }
            }
        }
    }

    void DestroyCurrentGun()
    {
        if (guns.Count > 0 && equippedGunIndex >= 0 && equippedGunIndex < guns.Count)
        {
            GameObject gunToDestroy = guns[equippedGunIndex];
            MeshRenderer renderer = gunToDestroy.GetComponent<MeshRenderer>();
            Collider collider = gunToDestroy.GetComponent<Collider>();

            if (renderer != null) renderer.enabled = true;  
            if (collider != null) collider.enabled = true;  

            guns.RemoveAt(equippedGunIndex);
            Destroy(gunToDestroy);
            equippedGunIndex = -1;
        }
    }

    void SwitchGun(int index)
    {
        if (ShopManager.Instance != null && ShopManager.Instance.shopUI.activeSelf)
        {
            return; 
        }
        StartCoroutine(SwitchGunWithDelay(index));
    }

    public void HideEquippedWeapons()
    {
        foreach (var gun in guns)
        {
            if (gun != null)
            {
                gun.SetActive(false);  
            }
        }
    }

    public void ShowEquippedWeapon()
    {
        if (equippedGunIndex >= 0 && equippedGunIndex < guns.Count)
        {
            SetActiveGun(guns[equippedGunIndex], true);
        }
    }

    public Weapon GetCurrentWeapon()
    {
        if (equippedGunIndex >= 0 && equippedGunIndex < guns.Count)
        {
            return guns[equippedGunIndex].GetComponent<Weapon>();
        }
        return null;
    }


}

