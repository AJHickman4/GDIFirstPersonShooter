using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool checkpointReached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.startingSpawn.transform.position != transform.position && checkpointReached == false)
        {
            checkpointReached = true;
            gameManager.instance.startingSpawn.transform.position = transform.position;
            StartCoroutine(menuPopUp());
        }
    }

    IEnumerator menuPopUp()
    {
        gameManager.instance.menuCheckpoint.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameManager.instance.menuCheckpoint.SetActive(false);
    }
}
