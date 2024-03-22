using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Shooting Parameters")]
    public bool isShooting;
    public bool readyToShoot = true;
    [Range(0, 3)] public float shootingDelay = 2f;
    private float timeSinceLastShot = 0f;
    public float reloadTime = 1f;
    public bool isReloading = false;
    
    [Header("Burst Fire Parameters")]
    [Range(1, 5)] public int bulletsPerBurst = 3;

    [Header("Spread Parameters")]
    [Range(0, 1)] public float spreadIntensity = 0.1f;

    [Header("Bullet Parameters")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    [Range(1, 70)] public float bulletVelocity = 30;
    [Range(1, 5)] public float bulletPrefabLife = 3f;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Shotgun Parameters")]
    [Range(1, 5)] public int pellets = 5;

    [Header("Ammo Parameters")]
    [Range(1, 30)] public int ammoPerMag = 30; // Ammo in each magazine
    public int currentAmmo;
    [Range(1, 5)] public int totalMags = 3; // Total number of magazines you can carry
    public int currentMags;

    [Header("Damage")]
    [Range(1, 30)] public int bulletDamage = 10;

    [Header("Equipment Status")]
    public bool isEquipped = false;

    [Header("Recoil Parameters")]
    [Range(1, 90)] public float recoilAmount = 2f;
    [Range(0, 10)] public float recoilTime = 0.1f;
    [Range(0, 10)] public float recoveryTime = 0.2f;

    private Quaternion originalRotation;
    private bool isRecoiling = false;
    public ParticleSystem muzzleFlash;

    public AudioSource gunShot;
    public AudioSource reload;
    
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
        originalRotation = transform.localRotation;
        gunShot = GetComponent<AudioSource>();
        readyToShoot = true;
        currentAmmo = ammoPerMag; // Initialize ammo count
        currentMags = totalMags; // Initialize magazine count
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;      
        if (isEquipped && !isReloading)
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
        
        if (isReloading) return;

        ShootBullet();
        currentAmmo--;
        timeSinceLastShot = 0f;
        gameManager.instance.UpdateAmmoUI(currentAmmo, currentMags, ammoPerMag, totalMags);

        if (mode == ShootingMode.Auto && !isRecoiling)
        {
            StartCoroutine(RecoilCoroutine(true));
        }
        else if (!isRecoiling)
        {
            StartCoroutine(RecoilCoroutine(false));
        }

        if (mode != ShootingMode.Auto)
        {
            readyToShoot = false;
            StartCoroutine(ResetShot(shootingDelay));
        }
    }

    IEnumerator FireBurst()
    {
        
        readyToShoot = false;
        if (!isRecoiling) // Check to prevent restarting the recoil
        {
            StartCoroutine(RecoilCoroutine(false));
        }
        for (int i = 0; i < bulletsPerBurst && currentAmmo > 0; i++)
        {
            if (isReloading) yield break;
            ShootBullet();
            currentAmmo--;
            yield return new WaitForSeconds(shootingDelay / bulletsPerBurst);
        }
        StartCoroutine(ResetShot(shootingDelay));
        gameManager.instance.UpdateAmmoUI(currentAmmo, currentMags, ammoPerMag, totalMags);
    }

    IEnumerator FireShotgun()
    {
        readyToShoot = false;
        if (!isRecoiling)
        {
            StartCoroutine(RecoilCoroutine(false));
        }

        gunShot.Play();
        muzzleFlash.Play();

        for (int i = 0; i < pellets && currentAmmo > 0; i++)
        {
            if (isReloading) yield break;
            ShootBullet(omitSound: true);
            currentAmmo--;
            yield return new WaitForSeconds(shootingDelay / pellets);
        }
        StartCoroutine(ResetShot(shootingDelay));
        gameManager.instance.UpdateAmmoUI(currentAmmo, currentMags, ammoPerMag, totalMags);
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break;
            isReloading = true;
        
        reload.Play();
        
        yield return new WaitForSeconds(reloadTime); 
        currentAmmo = ammoPerMag;
        if (currentMags > 0) currentMags--;
        isReloading = false;
        gameManager.instance.UpdateAmmoUI(currentAmmo, currentMags, ammoPerMag, totalMags);
    }

    void ShootBullet(bool omitSound = false)
    {

        if (!omitSound)
        {
            muzzleFlash.Play();
            gunShot.pitch = Random.Range(0.95f, 1.05f);
            gunShot.Play();
        }

        Vector3 shootingDirection = CalculateDirectionAndSpread();
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(shootingDirection));
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        Destroy(bullet, bulletPrefabLife);

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

    IEnumerator RecoilCoroutine(bool continuous)
    {
        isRecoiling = true;
        Quaternion startRotation = transform.localRotation;
        Quaternion recoilRotation = startRotation * Quaternion.Euler(-recoilAmount, 0, 0);
        if (continuous)
        {
            while (Input.GetButton("Shoot") && currentAmmo > 0)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, recoilRotation, recoilTime * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            float immediateRecoilTime = 0;
            while (immediateRecoilTime < recoilTime)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, recoilRotation, immediateRecoilTime / recoilTime);
                immediateRecoilTime += Time.deltaTime;
                yield return null;
            }
        }
        float timeElapsed = 0;
        Quaternion currentRotation = transform.localRotation;
        while (timeElapsed < recoveryTime)
        {
            timeElapsed += Time.deltaTime;
            float fraction = timeElapsed / recoveryTime;
            transform.localRotation = Quaternion.Slerp(currentRotation, startRotation, fraction);
            yield return null;
        }
        transform.localRotation = startRotation;
        isRecoiling = false;
    }
    public void AddOneMagIfNeeded()
    {
        if (currentMags == 0 && currentAmmo == 0)
        {
            currentAmmo = ammoPerMag;
            Debug.Log("Ammo added to the inventory.");
            gameManager.instance.UpdateAmmoUI(currentAmmo, currentMags, ammoPerMag, totalMags);
        }
        else
        {
            currentMags += 1;
            Debug.Log("One magazine added to the inventory.");
            gameManager.instance.UpdateAmmoUI(currentAmmo, currentMags, ammoPerMag, totalMags);
        }
    }
}