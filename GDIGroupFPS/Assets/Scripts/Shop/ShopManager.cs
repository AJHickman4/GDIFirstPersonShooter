using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static ShopItem;

public class ShopManager : MonoBehaviour
{
    public int credits;
    public TMP_Text creditsUI;
    public ShopItem[] shopItem;
    public GameObject[] shopPanels; //referencing the gameObject directly doesnt make me have to do .gameobject
    public ShopTemplate[] ShopPanels;
    public Button[] myPurchaseBtns;
    public GameObject shopUI;
    public GlobalWeaponsStatsManager weaponsStatsManager;
    public cameraController cameraController;
    public GameObject portal;
    public bool isShopOpen = false;

    //  public List<ShopItem> itemsForSale = new List<ShopItem>();
    public playerController player;

    public static ShopManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {

        for (int i = 0; i < shopItem.Length; i++)
        {
            shopPanels[i].SetActive(true);
        }

        creditsUI.text = "Credits: " + credits.ToString();

        loadPanels();
        UpdateCreditsDisplay();
        CheckPurchaseableItem();

        //list of items using their ShopItems script values. :) Add more items here as they exist. 
        //itemsForSale.Add(new ShopItem("Health Increase", 50, null, "Increase yer health by 20 permanently"));
        

    }

    private void Update()
    {
        

    }

    public void CheckPurchaseableItem()
    {

        for (int i = 0; i < shopItem.Length; i++)
        {
            myPurchaseBtns[i].interactable = gameManager.instance.playerScript.credits >= shopItem[i].cost;
        }

    }

    public void PurchaseItem(int btnNo)
    {
        if (gameManager.instance.playerScript.credits >= shopItem[btnNo].cost)
        {
            gameManager.instance.playerScript.credits -= shopItem[btnNo].cost;
            ApplyItemEffect(shopItem[btnNo]);
            UpdateCreditsDisplay();
            gameManager.instance.updateCreditsUI();
        }
    }


    private void ApplyItemEffect(ShopItem item)
    {
        switch (item.itemType)
        {
            case ItemType.AmmoCapacityUpgrade:
                GlobalWeaponsStatsManager.Instance.AddAmmoToReserve((int)item.effectValue);
                
                break;
            case ItemType.JumpUpgrade:
                if (player.jump < 2) 
                {
                    player.jump += item.effectValue;
                }
                break;
            case ItemType.SpeedUpgrade:
                player.speed += item.effectValue;
                player.sprintSpeed += item.effectValue;
                break;
            case ItemType.HealthUpgrade:
                player.HPOrig += (int)item.effectValue;
                player.HP = player.HPOrig;
                break;
            case ItemType.StaminaUpgrade:
                player.maxStamina += item.effectValue;
                player.currentStamina = player.maxStamina;
                break;
            case ItemType.PortalActivation:
                ActivatePortal();
                break;
            case ItemType.RefillaAmmo:
                GlobalWeaponsStatsManager.Instance.RefillAllWeaponsAmmo();
                break;
            case ItemType.teleport:
                gameManager.instance.AddEmergencyTeleport((int)item.effectValue);
                break;
            case ItemType.AmmoReturn:
                weaponsStatsManager.IncreaseAmmoReturnChanceForAllWeapons((int)item.effectValue);
                break;
            case ItemType.HealthRegen:
                player.PurchaseRegenerationBooster(); 
                break;
            case ItemType.staminaRegen:
                player.PurchaseStaminaRegenerationBooster();
                break;
        }
    }


    public void AddCoins()
    {
        //int amountToAdd = 10;
        gameManager.instance.playerScript.credits++; //players actual credits
                   //  credits += 5;//show of the shop credits //no
                   // creditsUI.text = "Credits: " + credits.ToString(); //no
        UpdateCreditsDisplay();
        
    }

   
   
    public void UpdateCreditsDisplay()
    {
        creditsUI.text = "Credits: " + gameManager.instance.playerScript.credits.ToString();
        CheckPurchaseableItem();
    }

    

    public void OpenShop()
    {
        isShopOpen = true;
        shopUI.SetActive(true);
        UpdateCreditsDisplay();
        GlobalWeaponsStatsManager.Instance.SetShootingDisabled(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cameraController.enabled = false;
    }

    public void CloseShop()
    {
        isShopOpen = false;
        shopUI.SetActive(false);
        GlobalWeaponsStatsManager.Instance.SetShootingEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraController.enabled = true;

    }

    public void loadPanels()
    {
        for (int i = 0; i < shopItem.Length; i++)
        {
            ShopPanels[i].titleTxt.text = shopItem[i].itemName;
            ShopPanels[i].description.text = shopItem[i].description;
            ShopPanels[i].costTxt.text = shopItem[i].cost.ToString() + " Credits: ";
        }
    }
    
    private void ActivatePortal()
    {
            portal.SetActive(true);  
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseShop();
        }
    }
}
