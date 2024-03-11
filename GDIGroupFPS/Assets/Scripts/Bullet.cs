using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public int maxBounces = 2; // Set the maximum number of bounces
    private int bounceCount = 0; // Counter for the number of bounces




    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Wall")
        {
            print("Hit" + collision.gameObject.name + "!");
            
            Destroy(gameObject);
        }   
        
        
        
        
        if (collision.gameObject.tag == "Target")
        {
            print("Hit" + collision.gameObject.name + "!");
            
            Destroy(gameObject);
        }

        bounceCount++;

        
        if (bounceCount >= maxBounces)
        {
            
            Destroy(gameObject);
        }
        
    }




}

