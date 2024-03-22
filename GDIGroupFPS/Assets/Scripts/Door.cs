using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] int lockValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        List<int> keys = gameManager.instance.playerScript.keys;


        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].Equals(lockValue))
            {
                gameObject.SetActive(false);
            }
        }
    }

}
