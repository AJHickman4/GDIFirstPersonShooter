using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closedoor : MonoBehaviour
{
    public Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !gameManager.instance.timerIsActive)
        {
            anim.SetTrigger("close");
        }
    }
}
