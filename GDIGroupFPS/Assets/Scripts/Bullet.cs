using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public BulletSource source;
    public ParticleSystem hitEffect;

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public enum BulletSource
    {
        Player,
        Enemy
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, transform.rotation).Play();
        }
        if (collision.gameObject.tag == "Enemy")
        {
            IDamage damageable = collision.gameObject.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(damage);
            }
        }
        else if (collision.gameObject.tag == "Breakable")
        {
            IDamage damageable = collision.gameObject.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(damage);
            }
        }
        Destroy(gameObject);
    }

}


