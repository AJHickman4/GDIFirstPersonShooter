using UnityEngine;

public class GlobalWeaponsStatsManager : MonoBehaviour
{
    public static GlobalWeaponsStatsManager Instance { get; private set; }

    
    public int additionalAmmoReserve = 0;

    public bool _canShoot = true;


    public event AmmoUpdateHandler OnAmmoAdded;
    public delegate void AmmoUpdateHandler(int ammoToAdd);
    public event ShootingUpdateHandler OnShootingUpdated;  
    public delegate void ShootingUpdateHandler(bool canShoot);

    public bool CanShoot
    {
        get { return _canShoot; }
        set
        {
            if (_canShoot != value)
            {
                _canShoot = value;
                OnShootingUpdated?.Invoke(_canShoot);
            }
        }
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void Start()
    {
        CanShoot = true;       
    }
    
    public void AddAmmoToReserve(int ammoToAdd)
    {
        additionalAmmoReserve += ammoToAdd;
        OnAmmoAdded?.Invoke(additionalAmmoReserve);  
        additionalAmmoReserve = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddAmmoToReserve(10);  
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetShootingEnabled(false);
        }   
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetShootingEnabled(true);
        }
    
    }

    public void SetShootingEnabled(bool enabled)
    {
        CanShoot = enabled;
    }

    public void SetShootingDisabled(bool disabled)
    {
        CanShoot = !disabled;
    }
}
