using UnityEngine;
using UnityEngine.UI;  
using System.Collections;
using TMPro;

public class DroneActivationTrigger : MonoBehaviour
{
    public FlyingDrone droneScript;
    public TMP_Text displayText; 
    public float displayDuration = 3.0f;  

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (droneScript != null)
            {
                droneScript.ToggleActivation();
                StartCoroutine(DisplayMessage("Drone model XR-7 online. Ready to soar, Captain!", displayDuration));
            }
        }
    }

    IEnumerator DisplayMessage(string message, float duration)
    {
        if (!displayText.gameObject.activeInHierarchy)
        {
            displayText.gameObject.SetActive(true);
        }
        displayText.text = message;  
        displayText.enabled = true;  
        yield return new WaitForSeconds(duration);
        displayText.enabled = false;  
        displayText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
