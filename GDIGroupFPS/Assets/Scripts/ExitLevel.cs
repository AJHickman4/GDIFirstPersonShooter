using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
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
        gameManager.instance.youHaveWon();
    }
}
