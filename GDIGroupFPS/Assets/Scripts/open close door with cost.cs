using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class DoorOpenClosewithcost : MonoBehaviour
{
    public Animator anim;
    public bool isPlayerAimingAtDoor = false;
    public bool isOpen = false;
    public int costToOpen = 10;
    public TextMeshProUGUI missingCreditsText; 

    void Start()
    {
        anim = GetComponent<Animator>();
        if (missingCreditsText != null)
            missingCreditsText.gameObject.SetActive(false); 
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
                if (missingCreditsText != null)
                    missingCreditsText.gameObject.SetActive(false); 
            }
            else if (player != null && player.credits < costToOpen)
            {
                int missingCredits = costToOpen - player.credits;
                if (missingCreditsText != null)
                {
                    missingCreditsText.text = $"Not enough credits. You need {missingCredits} more to open this door.";
                    missingCreditsText.gameObject.SetActive(true); 
                    StartCoroutine(HideMissingCreditsText());
                }
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

    IEnumerator HideMissingCreditsText()
    {
        yield return new WaitForSeconds(3f);
        if (missingCreditsText != null)
            missingCreditsText.gameObject.SetActive(false); 
    }
}
