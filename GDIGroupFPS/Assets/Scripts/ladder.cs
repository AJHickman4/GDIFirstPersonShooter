using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ladder : MonoBehaviour
{
    public Transform player;
    bool inside = false;
    public float speed = 100f;
    public float descentSpeed = 200f;
    public playerController input;
    private bool atBottom = false;


    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<playerController>();
        inside = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            input.enabled = false;
            inside = !inside;
        }
        else if (other.gameObject.tag == "LadderBottom")
        {
            // When reaching the bottom, allow walking again
            inside = false;
            input.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            input.enabled = true;
            inside = !inside;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inside == true && Input.GetKey("w"))
        {
            player.transform.position += Vector3.up / speed;
        }
        else //move down if not pressing W
        {
            player.transform.position -= Vector3.up * Time.deltaTime * (speed / descentSpeed);
        }
    }
}
