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
    public float baseRotationSpeed = 5f;
    public float visionConeAngle = 30f;
    public int health = 100;
    public Renderer model;
    public float flashDuration = 0.5f;

    private float lastShotTime;
    public AudioSource fireSound;
    public AudioSource die;
    private bool isDying = false;

    public float searchAngle = 45f; 
    public float searchSpeed = 1f; 
    private float searchDirection = 1f;
    private float currentSearchAngle = 0f;
    private Quaternion initialRotation;

    private float lostSightTime = 0f;
    public float lostSightDelay = 1.0f; 

    void Start()
    {
        initialRotation = partToRotate.localRotation;
        StartCoroutine(TargetSearchRoutine());
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;
        die?.Play();
        Destroy(gameObject, die.clip.length);
    }

    IEnumerator TargetSearchRoutine()
    {
        while (!isDying)
        {
            bool playerVisible = CheckForPlayerVisibility();
            if (playerVisible)
            {
                lostSightTime = 0f; 
            }
            else
            {
                if (lostSightTime >= lostSightDelay)
                {
                    PerformSearchBehavior();
                }
                else
                {
                    lostSightTime += Time.deltaTime; 
                }
            }
            yield return null;
        }
    }

    bool CheckForPlayerVisibility()
    {
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
                        if (hit.collider.CompareTag("Player"))
                        {
                            partToRotate.rotation = Quaternion.Slerp(partToRotate.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * baseRotationSpeed);
                            if (Time.time > lastShotTime + shootingInterval)
                            {
                                Shoot(hitCollider.transform.position);
                                lastShotTime = Time.time;
                            }
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    void PerformSearchBehavior()
    {
        currentSearchAngle += searchDirection * searchSpeed * Time.deltaTime;
        if (Mathf.Abs(currentSearchAngle) >= searchAngle)
        {
            searchDirection *= -1;
        }
        partToRotate.localRotation = initialRotation * Quaternion.Euler(0, currentSearchAngle, 0);
    }

    void Shoot(Vector3 targetPosition)
    {
        Instantiate(projectile, shootingPoint.position, Quaternion.LookRotation(targetPosition - shootingPoint.position));
        fireSound.Play();
    }
}