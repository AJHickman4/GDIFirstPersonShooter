using System.Collections;
using UnityEngine;
using System;
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
    [Range(1, 300)] public int Maxammo = 30; 
    public int currentAmmo;
    public int totalAmmoReserve;
    public int maxAmmoReserveLimit = 90;

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
    private int originalBulletDamage;
    private bool isDamageBoosted = false;
    public bool unlimitedAmmo = false;
    
    public static event Action OnDoubleDamageActivated;
    public static event Action OnDoubleDamageDeactivated;
    public static event Action OnShieldActivated;
    public static event Action OnShieldDeactivated;
    public static event Action OnUnlimitedAmmoActivated;
    public static event Action OnUnlimitedAmmoDeactivated;

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
        originalBulletDamage = bulletDamage; 
        originalRotation = transform.localRotation;
        gunShot = GetComponent<AudioSource>();
        readyToShoot = true;
        currentAmmo = Maxammo; // Initialize ammo count
        totalAmmoReserve -= currentAmmo;
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;      
        if (isEquipped && !isReloading)
        {
            if (Input.GetKeyDown(KeyCode.R))
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
        
        if (!unlimitedAmmo)
        {
            currentAmmo--;
        }
        

        timeSinceLastShot = 0f;
        gameManager.instance.UpdateAmmoUI(this.currentAmmo, this.totalAmmoReserve);

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
            if (!unlimitedAmmo) 
            {
                currentAmmo--;
            }
            yield return new WaitForSeconds(shootingDelay / bulletsPerBurst);
        }
        StartCoroutine(ResetShot(shootingDelay));
        gameManager.instance.UpdateAmmoUI(this.currentAmmo, this.totalAmmoReserve);
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
            if (!unlimitedAmmo)
            {
                currentAmmo--;
            }
            yield return new WaitForSeconds(shootingDelay / pellets);
        }
        StartCoroutine(ResetShot(shootingDelay));
        gameManager.instance.UpdateAmmoUI(this.currentAmmo, this.totalAmmoReserve);
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break;
        int ammoNeeded = Maxammo - currentAmmo;
        if (totalAmmoReserve > 0 && ammoNeeded > 0)
        {
            isReloading = true;
            reload.Play();
            yield return new WaitForSeconds(reloadTime);
            int ammoToLoad = Math.Min(ammoNeeded, totalAmmoReserve);
            currentAmmo += ammoToLoad;
            totalAmmoReserve -= ammoToLoad;

            gameManager.instance.UpdateAmmoUI(this.currentAmmo, this.totalAmmoReserve);

            isReloading = false;
        }
    }

    void ShootBullet(bool omitSound = false)
    {

        if (!omitSound)
        {
            muzzleFlash.Play();
            gunShot.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            gunShot.Play();
        }

        Vector3 shootingDirection = CalculateDirectionAndSpread();
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(shootingDirection));
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(bulletDamage);
        }
        
        Destroy(bullet, bulletPrefabLife);

    }

    IEnumerator ResetShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToShoot = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Vector3 spread = new Vector3(UnityEngine.Random.Range(-spreadIntensity, spreadIntensity), UnityEngine.Random.Range(-spreadIntensity, spreadIntensity), 0);
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
        Quaternion recoilRotation = startRotation * Quaternion.Euler(0, 0, recoilAmount); 
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
    public bool AddOneMagIfNeeded()
    {
        if (currentAmmo < Maxammo)
        {
            currentAmmo = Maxammo;
        }
        else
        {
            int potentialNewTotal = totalAmmoReserve + Maxammo;
            if (potentialNewTotal > maxAmmoReserveLimit)
            {
                totalAmmoReserve = maxAmmoReserveLimit;
            }
            else
            {
                totalAmmoReserve = potentialNewTotal;
                Debug.Log("Additional magazine added to reserve.");
            }
        }
        gameManager.instance.UpdateAmmoUI(this.currentAmmo, this.totalAmmoReserve);
        return true; 
    }

    public void ChangeDamage(int newDamage)
    {
        bulletDamage = newDamage;
    }

    public void ResetDamage()
    {
        bulletDamage = originalBulletDamage;
    }

    public void ApplyDamageBoost(int boostMultiplier, float duration)
    {
        if (!isDamageBoosted)
        {
            bulletDamage *= boostMultiplier;
            isDamageBoosted = true;
            OnDoubleDamageActivated?.Invoke();
            StartCoroutine(ResetDamageAfterDelay(duration));
        }
    }

    private IEnumerator ResetDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bulletDamage = originalBulletDamage;
        isDamageBoosted = false;
        OnDoubleDamageDeactivated?.Invoke();
    }
    public void EnableUnlimitedAmmo(float duration)
    {
        unlimitedAmmo = true;
        OnUnlimitedAmmoActivated?.Invoke();
        StartCoroutine(DisableUnlimitedAmmoAfterDelay(duration));
    }
    private IEnumerator DisableUnlimitedAmmoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        unlimitedAmmo = false;
        OnUnlimitedAmmoDeactivated?.Invoke();
    }
}

