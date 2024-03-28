using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitDoor : MonoBehaviour
{
    [SerializeField] int keysNeeded;
    int keysPlayerHas;
    int keysPlayerNeeds;

    // Start is called before the first frame update
    void Start()
    {
        keysPlayerNeeds = keysNeeded;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        keysPlayerHas = gameManager.instance.playerScript.keys.Count;

        if (keysNeeded == keysPlayerHas) // If player has the amount of keys needed
        {
            gameObject.SetActive(false);
        }
        else if (keysPlayerHas < keysNeeded) // Else if they don't, display how many they need via gameManager
        {
            gameManager.instance.exitDoorPrompt.SetActive(true);
            keysPlayerNeeds = keysNeeded - keysPlayerHas;
            gameManager.instance.updateKeysNeededUI(keysPlayerNeeds);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (gameManager.instance.exitDoorPrompt.activeInHierarchy)
            gameManager.instance.exitDoorPrompt.SetActive(false);
    }
}
