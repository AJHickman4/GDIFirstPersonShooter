using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipScript : MonoBehaviour
{
    [Header("Drag in")]
    public Transform PlayerTransform;
    public GameObject Gun;
    public Camera playerCamera;

    [Header("Range")]
    [Range(1, 5)]  public float equipRange = 5f;

    [Header("Audio")]
    public AudioSource audioSource; 
    public AudioClip pickupSound; 

    public bool isEquipped = false;
    private string weaponLayer = "weapons";
    private string defaultLayer = "Default";

    void Start()
    {
        Gun.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {

        if (Input.GetKeyDown("f"))
        {
            if (isEquipped)
            {
                UnequipObject();
            }
            else
            {
                TryEquipObjectWithRaycast();
            }
        }
    }

    void TryEquipObjectWithRaycast()
    {
        float sphereRadius = 0.5f;
        RaycastHit hit;
        if (Physics.SphereCast(playerCamera.transform.position, sphereRadius, playerCamera.transform.forward, out hit, equipRange))
        {
            if (hit.collider.gameObject.CompareTag("Gun"))
            {
                Gun = hit.collider.gameObject;
                EquipObject();
            }
        }
    }


    void EquipObject()
    {
        if (!isEquipped && Gun != null)
        {
            Gun.GetComponent<Rigidbody>().isKinematic = true;
            Gun.transform.position = PlayerTransform.position;
            Gun.transform.rotation = PlayerTransform.rotation;
            Gun.transform.SetParent(PlayerTransform);
            isEquipped = true;

            Weapon weaponScript = Gun.GetComponent<Weapon>();
            
            Gun.GetComponent<Weapon>().SetEquipped(true);
            Gun.layer = LayerMask.NameToLayer(weaponLayer);

            gameManager.instance.UpdateAmmoUI(weaponScript.currentAmmo,weaponScript.currentMags,weaponScript.ammoPerMag,weaponScript.totalMags);           
            audioSource.PlayOneShot(pickupSound);
        }
    }

    void UnequipObject()
    {
        Weapon weaponScript = Gun.GetComponent<Weapon>();

        if (weaponScript.unlimitedAmmo)
        {
            return; 
        }

        if (weaponScript.isReloading)
        {
            
            return;
        }

        if (isEquipped)
        {
            Gun.transform.SetParent(null);
            Gun.GetComponent<Rigidbody>().isKinematic = false;


            Gun.GetComponent<Rigidbody>().AddForce(PlayerTransform.forward * -0.5f + Vector3.down * 0.5f, ForceMode.VelocityChange);

            isEquipped = false;           
            Gun.GetComponent<Weapon>().SetEquipped(false);
            Gun.layer = LayerMask.NameToLayer(defaultLayer);
        }
    }
}