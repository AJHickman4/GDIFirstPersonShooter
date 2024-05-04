using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePlayerOnBelt : MonoBehaviour
{
    public bool inside;
    [SerializeField] float speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            gameManager.instance.playerScript.controller.Move(transform.right * speed * Time.deltaTime);



        }
        
    }
    
}
