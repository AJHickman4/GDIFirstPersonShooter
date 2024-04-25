using UnityEngine;

public class Rocketforturret : MonoBehaviour, IDamage
{
    public Transform target;  
    public float speed = 5f;  
    public float rotateSpeed = 200f;
    public int damage = 10;  
    public float hp = 10;
    public float explosionRadius = 5f;
    public ParticleSystem explosionEffect;
    public AudioSource explosionSound;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (target == null)
        target = GameObject.FindGameObjectWithTag("Player").transform;        
    }

    void Update()
    {
        if (target != null)
            Homing();
    }

    void Homing()
    {
        if (target == null)
            return;
        Collider playerCollider = target.GetComponent<Collider>();
        Vector3 targetCenter = playerCollider != null
            ? playerCollider.bounds.center
            : target.position + Vector3.up * (target.localScale.y / 2); 
        Vector3 direction = (targetCenter - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        rb.velocity = transform.forward * speed;
    }
    public void ApplyAoEDamage(Vector3 explosionPoint, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPoint, radius);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                IDamage damageable = hit.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.takeDamage(damage);
                }
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyAoEDamage(transform.position, explosionRadius);
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosionSound.Play();
            Die();
        }
    }

    public void takeDamage(int ammount)
    {
        hp -= ammount;
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
        Destroy(gameObject, explosionSound.clip.length);
    }
}
