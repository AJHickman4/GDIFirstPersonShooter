using UnityEngine;

public class RocketTurret : MonoBehaviour, IDamage
{
    public Transform target; 
    public GameObject rocketPrefab; 
    public Transform[] rocketSpawnPoints; 
    public float range = 50f; 
    public float fireRate = 1f; 
    public float health = 100f; 

    private float fireCooldown = 0f; 

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Goggles").transform;
    }

    void Update()
    {
        if (target != null && Vector3.Distance(target.position, transform.position) <= range)
        {
            if (fireCooldown <= 0f)
            {
                FireRockets();
                fireCooldown = 1f / fireRate;
            }
            fireCooldown -= Time.deltaTime;
        }
    }
    public void takeDamage(int ammount)
    {
        health -= ammount;
        if (health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    
    public void FireRockets()
    {
        foreach (Transform spawnPoint in rocketSpawnPoints)
        {
            Instantiate(rocketPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
