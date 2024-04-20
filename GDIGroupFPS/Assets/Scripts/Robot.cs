using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RobotFreeAnim : MonoBehaviour, IDamage
{
    [Header("Detection")]
    public float detectionRadius = 10f;  
    public float explosionDistance = 2f; 
    public LayerMask playerLayer;        
    public float rushSpeed = 10f;        

    [Header("Health")]
    public int health = 100;

    [Header("Explosion")]
    public GameObject explosionEffectPrefab;
    public float explosionRadius = 3f;
    public int explosionDamage = 100;
    public LayerMask explosionLayers;

    [Header("Audio")]
    public AudioClip explosionSound;
    public AudioSource audioSource;

    [Header("Rendering")]
    public MeshRenderer meshRenderer;  

    private NavMeshAgent agent;
    private Transform playerTransform;
    private bool hasExploded = false;
    private Collider myCollider; 

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        meshRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<Collider>(); 
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRadius && !hasExploded)
        {
            RushPlayer();
        }
        if (distanceToPlayer <= explosionDistance && !hasExploded)
        {
            Explode();
        }
    }

    private void RushPlayer()
    {
        agent.speed = rushSpeed;
        agent.SetDestination(playerTransform.position);
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void Explode()
    {
        hasExploded = true;
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        audioSource.PlayOneShot(explosionSound);
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        if (myCollider != null)
        {
            myCollider.enabled = false; 
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayers);
        foreach (Collider hit in colliders)
        {
            IDamage damageable = hit.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(explosionDamage);
            }
        }

        StartCoroutine(FadeOutAudio(4f));
        StartCoroutine(DestroyAfterSound(4f));
    }

    IEnumerator FadeOutAudio(float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Die();
    }
}