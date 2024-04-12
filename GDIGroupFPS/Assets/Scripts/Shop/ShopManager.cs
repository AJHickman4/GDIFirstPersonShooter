using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int credits;
    public TMP_Text creditsUI;



    public List<ShopItem> itemsForSale = new List<ShopItem>();
    public playerController player;

    void Start()
    {
        creditsUI.text = "Credits: " + credits.ToString();

        //list of items using their ShopItems script values. :) Add more items here as they exist. 
        itemsForSale.Add(new ShopItem("Health Increase", 50, null, "Increase yer health by 20 permanently"));


    }

    public void PurchaseItem(ShopItem item)
    {
        if (player.credits >= item.cost)
        {
            player.credits -= item.cost;
        }
        else
        {
            Debug.Log("Not enough Credits"); //TESTING, remove once working.
        }
    }

    private void ApplyItemEffect(ShopItem item)
    {
        //
    }


    public void AddCoins()
    {
        credits++;
        creditsUI.text = "Credits: " + credits.ToString();
    }

}
