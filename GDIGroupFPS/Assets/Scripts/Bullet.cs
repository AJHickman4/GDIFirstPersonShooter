using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public int maxBounces = 2; // Set the maximum number of bounces
    private int bounceCount = 0; // Counter for the number of bounces
    private int damage;

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }





    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            print("Hit " + collision.gameObject.name + "!");
            bounceCount++;
            if (bounceCount >= maxBounces)
            {
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            IDamage damageable = collision.gameObject.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(damage);
            }
            Destroy(gameObject);
        }
        else
        {
            bounceCount++;
            if (bounceCount >= maxBounces)
            {
                Destroy(gameObject);
            }
        }
    }




}

