using UnityEngine;

public class Rocket : MonoBehaviour
{
    public int damage;
    public float explosionRadius = 5f;
    public float launchForce = 1000f;
    public LayerMask damageableLayer;
    public ParticleSystem explosionEffect;
    private Rigidbody rocketRigidbody;

    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        if (rocketRigidbody != null)
        {
            LaunchRocket();
        }
    }

    void LaunchRocket()
    {
        Vector3 directionToCamera = (Camera.main.transform.position - transform.position).normalized;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToCamera);
        transform.rotation = desiredRotation;
        rocketRigidbody.AddForce(transform.forward * launchForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explosionEffect != null)
        {
            ParticleSystem explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosionInstance.Play();
            Destroy(explosionInstance.gameObject, explosionInstance.main.duration);
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayer);
        foreach (var hitCollider in hitColliders)
        {
            IDamage damageable = hitCollider.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(damage);
            }
        }
        Destroy(gameObject);
    }


    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
}
