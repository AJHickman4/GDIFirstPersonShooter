using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closedoortriggerwithcost : MonoBehaviour
{
    public Animator anim;
    public DoorOpenClosewithcost doorScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !gameManager.instance.timerIsActive)
        {
            if (doorScript != null)
            {
                anim.SetTrigger("close");
                doorScript.isOpen = false;
                doorScript.isPlayerAimingAtDoor = false;
            }
            else
            {
                //debug
            }
        }
    }
}
