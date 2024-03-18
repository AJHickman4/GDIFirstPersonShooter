using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Shooting Parameters")]
    public bool isShooting;
    public bool readyToShoot = true;
    public float shootingDelay = 2f;
    private float timeSinceLastShot = 0f;

    [Header("Burst Fire Parameters")]
    public int bulletsPerBurst = 3;

    [Header("Spread Parameters")]
    public float spreadIntensity = 0.1f;

    [Header("Bullet Parameters")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLife = 3f;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Shotgun Parameters")]
    public int pellets = 5;

    [Header("Ammo Parameters")]
    public int ammoPerMag = 30; // Ammo in each magazine
    public int currentAmmo;
    public int totalMags = 3; // Total number of magazines you can carry
    private int currentMags;

    [Header("Damage")]
    public int bulletDamage = 10;

    [Header("Equipment Status")]
    public bool isEquipped = false;

    
    AudioSource gunShot;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto,
        Shotgun
    }

    public ShootingMode mode;

    void Awake()
    {
        gunShot = GetComponent<AudioSource>();
        readyToShoot = true;
        currentAmmo = ammoPerMag; // Initialize ammo count
        currentMags = totalMags; // Initialize magazine count
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (isEquipped)
        {
            if (Input.GetKeyDown(KeyCode.R) && currentMags > 0 && currentAmmo < ammoPerMag)
            {
                StartCoroutine(Reload());
            }
            else if (readyToShoot && currentAmmo > 0)
            {
                switch (mode)
                {
                    case ShootingMode.Single:
                        if (Input.GetButtonDown("Shoot"))
                        {
                            FireWeapon();
                        }
                        break;
                    case ShootingMode.Burst:
                        if (Input.GetButtonDown("Shoot"))
                        {
                            StartCoroutine(FireBurst());
                        }
                        break;
                    case ShootingMode.Auto:
                        if (Input.GetButton("Shoot") && timeSinceLastShot >= shootingDelay)
                        {
                            FireWeapon();
                        }
                        break;
                    case ShootingMode.Shotgun:
                        if (Input.GetButtonDown("Shoot"))
                        {
                            StartCoroutine(FireShotgun());
                        }
                        break;
                }
            }
        }
    }

    private void FireWeapon()
    {
        ShootBullet();
        currentAmmo--;
        timeSinceLastShot = 0f; 
        if (mode != ShootingMode.Auto) 
        {
            readyToShoot = false;
            StartCoroutine(ResetShot(shootingDelay));
        }
    }

    IEnumerator FireBurst()
    {
        readyToShoot = false;
        for (int i = 0; i < bulletsPerBurst && currentAmmo > 0; i++)
        {
            ShootBullet();
            currentAmmo--;
            yield return new WaitForSeconds(shootingDelay / bulletsPerBurst);
        }
        StartCoroutine(ResetShot(shootingDelay));
    }

    IEnumerator FireShotgun()
    {
        readyToShoot = false;
        for (int i = 0; i < pellets && currentAmmo > 0; i++)
        {
            ShootBullet();
            currentAmmo--;
            yield return new WaitForSeconds(shootingDelay / pellets);
        }
        StartCoroutine(ResetShot(shootingDelay));
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(1f); // Reload time
        currentMags--;
        currentAmmo = ammoPerMag;
    }

    void ShootBullet()
    {
        Vector3 shootingDirection = CalculateDirectionAndSpread();
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(shootingDirection));
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        Destroy(bullet, bulletPrefabLife);
        gunShot.Play();
    }

    IEnumerator ResetShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToShoot = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Vector3 spread = new Vector3(Random.Range(-spreadIntensity, spreadIntensity), Random.Range(-spreadIntensity, spreadIntensity), 0);
        return playerCamera.transform.forward + spread;
    }

    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;
    }
}
