using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour, IDamage
{
    public GameObject projectile;
    public Transform shootingPoint;
    public Transform partToRotate;
    public Transform raycastOrigin;
    public float range = 10f;
    public float shootingInterval = 2f;
    public float rotationSpeed = 5f;
    public float visionConeAngle = 30f;
    public int health = 100;
    public Renderer model;
    public float flashDuration = 0.5f;

    private float lastShotTime;
    public AudioSource fireSound;
    public AudioSource die;
    private bool isDying = false;

    public float searchSpeed = 2f;
    public float searchAngle = 45f;
    private float searchDirection = 1f;
    private float currentSearchAngle = 0f;
    private float orientationFactor;
    private bool isResetting = false;
    private Quaternion initialRotation;

    private float timeSinceLastSawPlayer = Mathf.Infinity; 
    public float lostTargetDelay = 1.0f; 

    void Start()
    {
        initialRotation = partToRotate.localRotation;
        orientationFactor = Vector3.Dot(partToRotate.up, Vector3.up) > 0 ? 1f : -1f;
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Turret took damage. Current health: " + health);
        StartCoroutine(flashRed());
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;
        Debug.Log("Turret destroyed!");
        if (die != null && die.enabled)
        {
            die.Play();
            Destroy(gameObject, die.clip.length);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isDying) return;
        bool playerVisible = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Vector3 directionToPlayer = hitCollider.transform.position - raycastOrigin.position;
                float angle = Vector3.Angle(partToRotate.forward, directionToPlayer);
                if (angle < visionConeAngle)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(raycastOrigin.position, directionToPlayer.normalized, out hit, range))
                    {
                        Debug.DrawRay(raycastOrigin.position, directionToPlayer.normalized * range, Color.red);
                        if (hit.collider.CompareTag("Player"))
                        {
                            playerVisible = true;
                            timeSinceLastSawPlayer = 0; 
                            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                            partToRotate.rotation = Quaternion.Lerp(partToRotate.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                            if (Time.time > lastShotTime + shootingInterval)
                            {
                                Shoot(hitCollider.transform.position);
                                lastShotTime = Time.time;
                            }
                        }
                    }
                }
            }
        }

        if (!playerVisible)
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            if (timeSinceLastSawPlayer >= lostTargetDelay)
            {
                ResetToInitialRotation();
                if (isResetting)
                {
                    PerformSearchBehavior();
                }
            }
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        model.material.color = Color.white;
    }

    void Shoot(Vector3 targetPosition)
    {
        if (isDying) return;
        if (projectile && shootingPoint)
        {
            Instantiate(projectile, shootingPoint.position, Quaternion.LookRotation(targetPosition - shootingPoint.position));
            fireSound.Play();
        }
    }

    void PerformSearchBehavior()
    {
        if (isResetting)
        {
            currentSearchAngle += searchDirection * searchSpeed * Time.deltaTime;
            currentSearchAngle = Mathf.Clamp(currentSearchAngle, -searchAngle, searchAngle);
            partToRotate.localRotation = initialRotation * Quaternion.AngleAxis(currentSearchAngle * orientationFactor, Vector3.up);
            if (Mathf.Abs(currentSearchAngle) >= searchAngle)
            {
                searchDirection *= -1;
            }
        }
    }

    void ResetToInitialRotation()
    {
        if (Quaternion.Angle(partToRotate.localRotation, initialRotation) > 0.1f)
        {
            partToRotate.localRotation = Quaternion.Slerp(partToRotate.localRotation, initialRotation, Time.deltaTime * rotationSpeed);
        }
        else if (!isResetting)
        {
            partToRotate.localRotation = initialRotation;
            isResetting = true;
            currentSearchAngle = 0f;
        }
    }
}