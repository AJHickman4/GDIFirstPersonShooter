using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int credits;
    public TMP_Text creditsUI;
    public ShopItem[] shopItem;
    public GameObject[] shopPanels; //referencing the gameObject directly doesnt make me have to do .gameobject
    public ShopTemplate[] ShopPanels;
    public Button[] myPurchaseBtns;




  //  public List<ShopItem> itemsForSale = new List<ShopItem>();
    public playerController player;

    void Start()
    {
        for (int i = 0; i < shopItem.Length; i++)
        {
            shopPanels[i].SetActive(true);
        }

        creditsUI.text = "Credits: " + player.credits;
        loadPanels();
        CheckPurchaseableItem();
        //list of items using their ShopItems script values. :) Add more items here as they exist. 
       //itemsForSale.Add(new ShopItem("Health Increase", 50, null, "Increase yer health by 20 permanently"));


    }

    public void CheckPurchaseableItem()
    {
        for (int i = 0; i < shopItem.Length; i++)
        {
            if (player.credits >= shopItem[i].cost)//if the player doesnt have enough money.
            {
                myPurchaseBtns[i].interactable = true;
            }
            else
            {
                myPurchaseBtns[i].interactable = false;
            }
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (player.credits >= shopItem[btnNo].cost)
        {
            player.credits -= shopItem[btnNo].cost;


            UpdateCreditsDisplay();
            //Unlock Item insert here when ready :)
        }
        else
        {
            Debug.Log("Not Enough Creds");
        }
    }
    

    private void ApplyItemEffect()
    {
        //
    }
    

    public void AddCoins()
    {
        player.credits += 5; //players actual credits
        //creditsUI.text = "Credits: " + credits.ToString();
        UpdateCreditsDisplay();
    }
    public void UpdateCreditsDisplay()
    {
        creditsUI.text = "Credits: " + player.credits;
        CheckPurchaseableItem();
    }

    public void loadPanels() 
    { 
        for (int i = 0 ; i < shopItem.Length; i++)
        {
            ShopPanels[i].titleTxt.text = shopItem[i].itemName;
            ShopPanels[i].description.text = shopItem[i].description;
            ShopPanels[i].costTxt.text = shopItem[i].cost.ToString() + "Credits: ";
        } 
    }
}
