using UnityEngine;

public class FlyingDrone: MonoBehaviour
{
    public Transform target; 
    public float orbitDistance = 5f; 
    public float followHeight = 2f; 
    public float enemyKeepDistance = 7f; 
    public float shootingRange = 10f; 
    public GameObject bulletPrefab;
    public Transform bulletSpawn; 
    public float fireRate = 1f; 
    public float movementSpeed = 5f; 
    public AudioSource fireSound;
    public float orbitSpeed = 30f; 

    private float lastFireTime;
    private Transform currentAttackTarget;
    private float orbitAngle = 0f; 
    public bool isActive = false; 

    void Update()
    {
        if (isActive && target != null)
        {
            OrbitAroundTarget();
            PerformAttack();
        }
    }

    public void InitializeDrone(Transform playerTransform)
    {
        target = playerTransform; 
        lastFireTime = Time.time;
    }

    public void ToggleActivation()
    {
        isActive = true;
        gameObject.SetActive(isActive);
    }
    public void ToggleDeactivation()
    {
        isActive = false;
        gameObject.SetActive(isActive);
    }   
    private void OrbitAroundTarget()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        Vector3 orbitDirection = new Vector3(Mathf.Sin(orbitAngle), 0, Mathf.Cos(orbitAngle));
        Vector3 followPosition = target.position + Vector3.up * followHeight + orbitDirection * orbitDistance;

        if (currentAttackTarget != null)
        {
            Vector3 directionToEnemy = (currentAttackTarget.position - target.position).normalized;
            Vector3 enemyAvoidancePosition = currentAttackTarget.position - directionToEnemy * enemyKeepDistance;
            followPosition = Vector3.Lerp(followPosition, enemyAvoidancePosition, 0.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentAttackTarget.position - transform.position), Time.deltaTime * movementSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.forward), Time.deltaTime * movementSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, followPosition, movementSpeed * Time.deltaTime);
    }

    private void PerformAttack()
    {
        currentAttackTarget = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootingRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                currentAttackTarget = hitCollider.transform;
                if (Time.time > lastFireTime + 1f / fireRate)
                {
                    ShootAtTarget(currentAttackTarget.position);
                    lastFireTime = Time.time;
                    break; 
                }
            }
        }
    }

    private void ShootAtTarget(Vector3 targetPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(targetPosition - bulletSpawn.position));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = (targetPosition - bulletSpawn.position).normalized * 20f; 
        }
        fireSound.Play();
    }
}