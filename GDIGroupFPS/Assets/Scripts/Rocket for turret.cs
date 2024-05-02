using UnityEngine;

public class RocketForTurret : MonoBehaviour, IDamage
{
    public Transform target;
    public float speed = 5f;
    public float rotateSpeed = 200f;
    public int damage = 10;
    public float hp = 10;
    public float explosionRadius = 5f;
    public float collisionIgnoreTime = 2f;  
    public ParticleSystem explosionEffect;
    public AudioSource explosionSound;

    private Rigidbody rb;
    private Collider coll;
    private bool canExplode = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        coll.enabled = false; 
        Invoke(nameof(EnableCollider), collisionIgnoreTime); 
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        Destroy(gameObject, 15f);  
    }

    void Update()
    {
        if (target != null)
        {
            Homing();
        }
    }

    void Homing()
    {
        if (target == null) return;

        Vector3 targetCenter = target.position + Vector3.up * (target.localScale.y / 2);
        Vector3 direction = (targetCenter - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        rb.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canExplode) return; 

        Explode();
    }

    void EnableCollider()
    {
        coll.enabled = true; 
        canExplode = true; 
    }

    void Explode()
    {
        ApplyAoEDamage(transform.position, explosionRadius);
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        if (explosionSound != null)
        {
            explosionSound.Play();
        }
        Die();
    }

    public void ApplyAoEDamage(Vector3 explosionPoint, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPoint, radius);
        foreach (Collider hit in colliders)
        {
            IDamage damageable = hit.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(damage);
            }
        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        Destroy(gameObject, explosionSound != null ? explosionSound.clip.length : 0f);
    }
}