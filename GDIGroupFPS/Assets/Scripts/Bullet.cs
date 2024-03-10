using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
   private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            print("Hit" + collision.gameObject.name + "!");
            
            Destroy(gameObject);
        }
    }
}
