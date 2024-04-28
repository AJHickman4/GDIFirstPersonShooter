using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltEffect : MonoBehaviour
{
    public float speed = 0.5f;
    [SerializeField] bool faceForward = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && !faceForward)
        {
            other.attachedRigidbody.MovePosition(other.transform.position + Vector3.right * speed * Time.deltaTime);
        }
        else if (other.attachedRigidbody && faceForward)
        {
            other.attachedRigidbody.MovePosition(other.transform.position + Vector3.forward * speed * Time.deltaTime);
        }
    }
}
