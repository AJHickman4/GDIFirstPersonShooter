using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipScript : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject Gun;
    private bool isEquipped = false; 

  

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
                EquipObject();
            }
        }
    }

    void EquipObject()
    {
        if (!isEquipped) 
        {
            Gun.GetComponent<Rigidbody>().isKinematic = true;
            Gun.transform.position = PlayerTransform.position; 
            Gun.transform.rotation = PlayerTransform.rotation;
            Gun.transform.SetParent(PlayerTransform);
            isEquipped = true;
        }
    }

    void UnequipObject()
    {
        if (isEquipped) 
        {
            PlayerTransform.DetachChildren();
            Gun.GetComponent<Rigidbody>().isKinematic = false;

            
            Gun.transform.eulerAngles = new Vector3(Gun.transform.eulerAngles.x, Gun.transform.eulerAngles.y, Gun.transform.eulerAngles.z - 45);

            isEquipped = false;
        }
    }
}