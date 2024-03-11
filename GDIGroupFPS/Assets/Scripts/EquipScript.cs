using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipScript : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject Gun;
    public bool isEquipped = false;
    public Camera playerCamera;
    public float equipRange = 5f;
    
    
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
            Gun.GetComponent<Weapon>().SetEquipped(true);
        }
    }

    void UnequipObject()
    {
        if (isEquipped)
        {
            Gun.transform.SetParent(null);
            Gun.GetComponent<Rigidbody>().isKinematic = false;


            Gun.GetComponent<Rigidbody>().AddForce(PlayerTransform.forward * -0.5f + Vector3.down * 0.5f, ForceMode.VelocityChange);

            isEquipped = false;
            Gun.GetComponent<Weapon>().SetEquipped(false);
        }
    }
}