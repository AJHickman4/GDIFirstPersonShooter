using System.Collections;
using UnityEngine;
using System;
//using System.Diagnostics;
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
    public float ammoReturnChance = 0f; // chance to return ammo after shooting bought from shop

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Shotgun Parameters")]
    [Range(1, 5)] public int pellets = 5;

    [Header("Ammo Parameters")]
    [Range(1, 300)] public int Maxammo = 30; 
    public int currentAmmo;
    public int totalAmmoReserve;
    public int maxAmmoReserveLimit = 90;
    public int maxCurrentAmmo; // orginal max ammo
    public int maxTotalAmmoReserve; // orginal max ammo reserve

    [Header("Damage")]
    [Range(1, 100)] public int bulletDamage = 10;

    [Header("Equipment Status")]
    public bool isEquipped = false;

    [Header("Recoil Parameters")]
    [Range(1, 90)] public float recoilAmount = 2f;
    [Range(0, 10)] public float recoilTime = 0.1f;
    [Range(0, 10)] public float recoveryTime = 0.2f;

    private Quaternion originalRotation;
    public bool isRecoiling = false;
    public ParticleSystem muzzleFlash;
    public playerController PlayerController;

    public AudioSource gunShot;
    public AudioSource reload;
    public bool unlimitedAmmo = false;
    public bool canShoot = true;
    public Weapon currentWeapon;
    public Animator anim;
    public Vector3 originalPosition;
    public Vector3 loweredPosition;

    [SerializeField] private GlobalWeaponsStatsManager globalWeaponsStatsManager;

    public Transform gunTransform;
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto,
        Shotgun
    }
    void Start()
    {
        ApplyGlobalAmmoReserve();

        loweredPosition = new Vector3(originalPosition.x, originalPosition.y +0.15f, originalPosition.z); // Adjust as needed
    }
    
    public ShootingMode mode;

    void Awake()
    {
        originalRotation = transform.localRotation;
        gunShot = GetComponent<AudioSource>();
        PlayerController = FindObjectOfType<playerController>();
        readyToShoot = true;
        currentAmmo = Maxammo; // Initialize ammo count
        totalAmmoReserve -= currentAmmo;
    }

    void Update()
    {
        
        timeSinceLastShot += Time.deltaTime;      
        if (isEquipped && !isReloading && canShoot)
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
        
        if (gameManager.instance.isPaused)
        {
            return; 
        }
        if (!unlimitedAmmo && UnityEngine.Random.value < ammoReturnChance / 100.0f) 
        {
            currentAmmo++;
            if (isEquipped)
            {
                gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
            }
        }

        if (isReloading || !canShoot) return;
          
        ShootBullet();

        if (!PowerUpManager.Instance.HasUnlimitedAmmo)
        {
            currentAmmo--;
        }
        

        timeSinceLastShot = 0f;
        if (isEquipped)
        {
            gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
        }

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
        if (gameManager.instance.isPaused)
        {
            yield break; 
        }
        if (!unlimitedAmmo && UnityEngine.Random.value < ammoReturnChance / 100.0f) 
        {
            currentAmmo++;
            if (isEquipped)
            {
                gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
            }
        }
        if (!canShoot || isReloading) yield break;
        readyToShoot = false;
        if (!isRecoiling) 
        {
            StartCoroutine(RecoilCoroutine(false));
        }
        for (int i = 0; i < bulletsPerBurst && (PowerUpManager.Instance.HasUnlimitedAmmo || currentAmmo > 0); i++)
        {
            if (isReloading) yield break;
            ShootBullet();
            if (!PowerUpManager.Instance.HasUnlimitedAmmo)
            {
                currentAmmo--;
            }
            yield return new WaitForSeconds(shootingDelay / bulletsPerBurst);
        }
        StartCoroutine(ResetShot(shootingDelay));
        if (isEquipped)
        {
            gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
        }
    }

    IEnumerator FireShotgun()
    {
        if (gameManager.instance.isPaused)
        {
            yield break; 
        }
        if (!unlimitedAmmo && UnityEngine.Random.value < ammoReturnChance / 100.0f) 
        {
            currentAmmo++;
            if (isEquipped)
            {
                gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
            }
        }
        if (!canShoot || isReloading) yield break;
        readyToShoot = false;
        if (!isRecoiling)
        {
            StartCoroutine(RecoilCoroutine(false));
        }

        gunShot.Play();
        muzzleFlash.Play();

        for (int i = 0; i < pellets && (PowerUpManager.Instance.HasUnlimitedAmmo || currentAmmo > 0); i++)
        {
            if (isReloading) yield break;
            ShootBullet(omitSound: true);
            if (!PowerUpManager.Instance.HasUnlimitedAmmo)
            {
                currentAmmo--;
            }
            yield return new WaitForSeconds(shootingDelay / pellets);
        }
        StartCoroutine(ResetShot(shootingDelay));
        if (isEquipped)
        {
            gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
        }
    }

    public IEnumerator Reload()
    {
        if (isReloading) yield break;
        isReloading = true;
        canShoot = false;
        int ammoNeeded = Maxammo - currentAmmo;
        if (totalAmmoReserve > 0 && ammoNeeded > 0)
        {
            isReloading = true;
            reload.Play();
            StartCoroutine(LowerGun());
            yield return new WaitForSeconds(reloadTime);
            StartCoroutine(RaiseGun());
            int ammoToLoad = Math.Min(ammoNeeded, totalAmmoReserve);
            currentAmmo += ammoToLoad;
            totalAmmoReserve -= ammoToLoad;
            if (isEquipped)
            {
                gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
            }
            readyToShoot = true;
            canShoot = true;
            isReloading = false;
        }
    }

    private IEnumerator LowerGun()
    {
        
        float duration = 0.2f; 
        float elapsed = 0;
        while (elapsed < duration)
        {
            gunTransform.localPosition = Vector3.Lerp(originalPosition, loweredPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = loweredPosition;
    }

    private IEnumerator RaiseGun()
    {
        float duration = 0.2f; 
        float elapsed = 0;
        while (elapsed < duration)
        {
            gunTransform.localPosition = Vector3.Lerp(loweredPosition, originalPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gunTransform.localPosition = originalPosition;
    }

void ShootBullet(bool omitSound = false)
    {

        if (gameManager.instance.isPaused)
        {
            return; // Do not proceed with shooting if the game is paused
        }



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
            int finalDamage = bulletDamage;
            if (PowerUpManager.Instance.HasDoubleDamage)
            {
                finalDamage *= 2;
            }
            bulletScript.SetDamage(finalDamage);
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
        currentWeapon = this;
    }

    IEnumerator RecoilCoroutine(bool continuous)
    {
        isRecoiling = true;
        Quaternion startRotation = transform.localRotation;
        Quaternion recoilRotation = startRotation * Quaternion.Euler(0, 0, recoilAmount);

        if (continuous)
        {
            while (Input.GetButton("Shoot") && currentAmmo > 0 && PlayerController.isMeleeReady && !isReloading)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, recoilRotation, recoilTime * Time.deltaTime);
                yield return null;
                if (isReloading)
                {
                    transform.localRotation = startRotation;
                    isRecoiling = false;
                    yield break;
                }
            }
        }
        else
        {
            float immediateRecoilTime = 0;
            while (immediateRecoilTime < recoilTime && PlayerController.isMeleeReady && !isReloading)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, recoilRotation, immediateRecoilTime / recoilTime);
                immediateRecoilTime += Time.deltaTime;
                yield return null;
                if (isReloading)
                {
                    transform.localRotation = startRotation;
                    isRecoiling = false;
                    yield break;
                }
            }
        }

        if (PlayerController.isMeleeReady && !isReloading)
        {
            float timeElapsed = 0;
            Quaternion currentRotation = transform.localRotation;
            while (timeElapsed < recoveryTime)
            {
                timeElapsed += Time.deltaTime;
                float fraction = timeElapsed / recoveryTime;
                transform.localRotation = Quaternion.Slerp(currentRotation, startRotation, fraction);
                yield return null;
                if (isReloading)
                {
                    transform.localRotation = startRotation;
                    isRecoiling = false;
                    yield break;
                }
            }
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
            }
        }
        if (isEquipped)
        {
            gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
        }
        return true; 
    }

    void ApplyGlobalAmmoReserve()
    {
        if (GlobalWeaponsStatsManager.Instance != null)
        {
            totalAmmoReserve += GlobalWeaponsStatsManager.Instance.additionalAmmoReserve;
            
        }
        else
        {
            //Debug.LogError("GlobalWeaponsStatsManager instance not found.");
        }
    }
    void OnEnable()
    {
        globalWeaponsStatsManager.OnAmmoAdded += UpdateAmmoReserve;
        globalWeaponsStatsManager.OnShootingUpdated += UpdateCanShoot;
        globalWeaponsStatsManager.OnAmmoAdded += UpdateRefillAllAmmo;
        
    }

    void OnDisable()
    {
        globalWeaponsStatsManager.OnAmmoAdded -= UpdateAmmoReserve;
        globalWeaponsStatsManager.OnShootingUpdated -= UpdateCanShoot;
        globalWeaponsStatsManager.OnAmmoAdded -= UpdateRefillAllAmmo;
    }
    private void UpdateAmmoReserve(int ammoToAdd)
    {
        maxAmmoReserveLimit += ammoToAdd;  
    }

    private void UpdateCanShoot(bool canShoot)
    {
        this.canShoot = canShoot;
    }
    private void UpdateRefillAllAmmo(int ammoToAdd)
    {
        RefillAllAmmo();
    }
    public void RefillAllAmmo()
    {
        currentAmmo = maxCurrentAmmo;
        int ammoToAdd = Math.Min(maxTotalAmmoReserve, maxAmmoReserveLimit - totalAmmoReserve);
        totalAmmoReserve += ammoToAdd;
        if (isEquipped)
        {
            gameManager.instance.UpdateAmmoUI(currentAmmo, totalAmmoReserve);
        }
    }
}

