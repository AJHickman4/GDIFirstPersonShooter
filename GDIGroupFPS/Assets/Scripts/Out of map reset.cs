using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outofmapreset : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.spawnPlayer();
            gameManager.instance.StopAllCoroutines();
            gameManager.instance.teleportEffect.Clear();
            gameManager.instance.teleportEffect.Stop();
        }
    }
}
