using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class key : MonoBehaviour
{
    [SerializeField] int keyValue;
    bool alreadyHasKey;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 30) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (gameManager.instance.playerScript.keys.Count != 0)
            {
                for (int i = 0; i < gameManager.instance.playerScript.keys.Count; i++)
                {
                    if (gameManager.instance.playerScript.keys[i] == keyValue)
                    {
                        alreadyHasKey = true;
                        Debug.Log("player already has key");
                    }
                    else
                    {
                        alreadyHasKey = false;
                        Debug.Log("key not in list");
                    }
                    Debug.Log(gameManager.instance.playerScript.keys[i]);
                    Debug.Log("keyValue=" + keyValue);
                }
            }
            else
            {
                alreadyHasKey = false;
                Debug.Log("player has no keys");
            }

            if (!alreadyHasKey)
            {
                gameManager.instance.playerScript.keys.Add(keyValue);
                gameManager.instance.updateKeyUI();
                gameObject.SetActive(false);
            }
        }
    }
}
