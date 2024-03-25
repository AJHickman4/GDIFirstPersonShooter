using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class shopUI : slotHolderUI<inventoryItem>
{
    [SerializeField] private shopSign shopsign = null;
    private shop shop = null;

    [SerializeField] private TextMeshProUGUI titleOfShop = null;

    void Start()
    {
        shopsign.onEnterShop += SetShop;

        shopsign.onExitShop += ExitShop;
    }

    private void SetShop(shop shop)
    {
        this.shop = shop;

        titleOfShop.text = shop.name;

        //InitializeUI(shop.GetInventory, shop.MaxItems);

        //ChangeInventoryState();

        //UpdateItemSlots();

        //shop.onInventoryChange += UpdateItemSlots;
    }

    private void ExitShop()
    {
        //shop.onInventoryChange -= UpdateItemSlots;
        //ChangeInventoryState();
    }
}