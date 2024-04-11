using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    private Animator anim;
    public bool isPlayerAimingAtDoor = false; 
    public bool isOpen = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerAimingAtDoor && !isOpen && !gameManager.instance.timerIsActive)
        {
            anim.SetTrigger("open");
            isOpen = true; 
        }
    }
    public void SetPlayerAimingAtDoor(bool isAiming)
    {
        isPlayerAimingAtDoor = isAiming;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.SetTrigger("close");
            isOpen = false; 
        }
    }
}


