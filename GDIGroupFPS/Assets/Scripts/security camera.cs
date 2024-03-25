using UnityEngine;

public class EnemyDetectionCamera : MonoBehaviour, IDamage
{
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 30f;
    public float rotationSpeed = 5f;
    public Transform partToRotate;
    public float searchSpeed = 2f;
    public float searchAngle = 45f;
    private float searchDirection = 1f;
    private float currentSearchAngle = 0f;
    private Quaternion initialRotation;
    public int health = 100;
    public ParticleSystem detectionEffect;
    public ParticleSystem searchEffect; 
    public Transform raycastOrigin;
    public AudioClip detectionSound; 
    public AudioClip searchSound; 
    private AudioSource audioSource; 

    private bool isPlayerDetected = false;

    void Start()
    {
        initialRotation = partToRotate.localRotation;
        InvokeRepeating("DetectTargets", 0f, 0.5f);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Missing AudioSource component.");
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
    }


    void Update()
    {
        PerformSearchBehavior();
    }

    void DetectTargets()
    {
        if (raycastOrigin == null)
        {
            return;
        }

        bool playerDetected = false;
        float halfFOV = fieldOfViewAngle / 2.0f;
        float sphereRadius = 0.5f; 
        for (float angle = -halfFOV; angle <= halfFOV; angle += halfFOV / 10)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * raycastOrigin.forward;
            RaycastHit hit;
            if (Physics.SphereCast(raycastOrigin.position, sphereRadius, direction, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerDetected = true;
                    Debug.Log("Player detected: " + hit.collider.name);
                    // add logic here to handle player detection here
                    
                    
                    
                    Debug.DrawRay(raycastOrigin.position, direction * hit.distance, Color.red);
                    break;
                }
            }
        }
        HandleParticleEffects(playerDetected);
    }

    void HandleParticleEffects(bool playerDetected)
    {
        if (playerDetected)
        {
            if (!isPlayerDetected)
            {
                isPlayerDetected = true;
                searchEffect?.Stop();
                detectionEffect?.Play();
                audioSource.clip = detectionSound; 
                audioSource.loop = true; 
                audioSource.Play(); 
            }
        }
        else
        {
            if (isPlayerDetected)
            {
                isPlayerDetected = false;
                detectionEffect?.Stop();
                searchEffect?.Play();
                audioSource.clip = searchSound; 
                audioSource.loop = false; 
                audioSource.Play(); 
            }
        }
    }
    
    void PerformSearchBehavior()
    {
        if (!isPlayerDetected)
        {
            currentSearchAngle += searchDirection * searchSpeed * Time.deltaTime;
            currentSearchAngle = Mathf.Clamp(currentSearchAngle, -searchAngle, searchAngle);
            partToRotate.localRotation = initialRotation * Quaternion.AngleAxis(currentSearchAngle, Vector3.up);
            if (Mathf.Abs(currentSearchAngle) >= searchAngle)
            {
                searchDirection *= -1;
            }
        }
    }
    
    public void takeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"Camera took {amount} damage, remaining health: {health}");
        if (health <= 0)
        {
            Die(); 
        }
    }

    private void Die()
    {
        Debug.Log("Camera destroyed!");
        Destroy(gameObject);
    }


}