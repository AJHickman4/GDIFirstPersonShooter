using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenClosecost : MonoBehaviour
{
    public Animator anim;  
    public bool isPlayerAimingAtDoor = false;
    public bool isOpen = false;
    public int costToOpen = 10;  

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerAimingAtDoor && !isOpen && !gameManager.instance.timerIsActive)
        {
            playerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
            if (player != null && player.credits >= costToOpen)
            {
                anim.SetTrigger("open");
                isOpen = true;
                player.credits -= costToOpen; 
                gameManager.instance.updateCreditsUI();  
            }
            else if (player != null && player.credits < costToOpen)
            {
                //Debug.Log("Not enough credits to open the door."); use to tell the player they don't have enough credits
            }
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
