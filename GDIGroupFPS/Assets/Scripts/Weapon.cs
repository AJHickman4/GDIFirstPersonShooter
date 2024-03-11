using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Shooting parameters
    public bool isShooting;
    public bool readyToShoot;
    private bool allowReset = true;
    public float shootingDelay = 2f;

    // Time tracking for auto firing delay
    private float timeSinceLastShot = 0f;

    // Burst fire parameters
    public int bulletsPerBurst = 3;
    private int currentBurst;

    // Spread parameters
    public float spreadIntensity = 0.1f;

    // Bullet parameters
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLife = 3f;
    public Camera playerCamera;

    
    
    
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode mode;

    void Awake()
    {
        readyToShoot = true;
        currentBurst = bulletsPerBurst;
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime; // Increment the time since the last shot

        switch (mode)
        {
            case ShootingMode.Single:
                if (Input.GetButtonDown("Shoot") && readyToShoot)
                {
                    FireWeapon();
                }
                break;
            case ShootingMode.Burst:
                if (Input.GetButtonDown("Shoot") && readyToShoot)
                {
                    StartCoroutine(FireBurst());
                }
                break;
            case ShootingMode.Auto:
                if (Input.GetButton("Shoot") && readyToShoot && timeSinceLastShot >= shootingDelay)
                {
                    FireWeapon();
                }
                break;
        }
    }

    private void FireWeapon()
    {
        if (!readyToShoot) return;

        readyToShoot = false;
        ShootBullet();

        if (mode == ShootingMode.Auto)
        {
            timeSinceLastShot = 0f; // Reset the timer for auto mode
        }

        if (mode != ShootingMode.Burst)
        {
            StartCoroutine(ResetShot());
        }
    }

    void ShootBullet()
    {
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(shootingDirection));
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLife));
    }

    IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    IEnumerator FireBurst()
    {
        allowReset = false;
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            if (readyToShoot)
            {
                ShootBullet();
                yield return new WaitForSeconds(shootingDelay / bulletsPerBurst);
            }
        }
        StartCoroutine(ResetShot());
    }

    IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(shootingDelay);
        readyToShoot = true;
        allowReset = true;
        currentBurst = bulletsPerBurst; // Reset burst count after a delay
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Vector3 spread = new Vector3(Random.Range(-spreadIntensity, spreadIntensity), Random.Range(-spreadIntensity, spreadIntensity), 0);
        Vector3 shootingDirection = playerCamera.transform.forward + spread; // Add spread to the forward direction
        return shootingDirection;
    }
}
