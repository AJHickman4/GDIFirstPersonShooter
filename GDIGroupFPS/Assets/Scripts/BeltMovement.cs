using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltEffect : MonoBehaviour
{
    public float speed = 0.5f; 

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            other.attachedRigidbody.MovePosition(other.transform.position + Vector3.right * speed * Time.deltaTime);
        }
    }
}
