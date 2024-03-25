using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shop : inventory
{
    private shopSign shopsign = null;

    [SerializeField] private List<inventoryItem> shopItems = new List<inventoryItem>();
    [SerializeField] private int numberToAdd = 5;

    private int maxItems = 6;
    public int MaxItems => maxItems;

    public delegate void OnEnterShop();
    public OnEnterShop onEnterShop;

    public delegate void OnExitShop();
    public OnExitShop onExitShop;

    private void Start()
    {
        //foreach (inventoryItem i in shopItems)
            //AddToInventory(i, numberToAdd);
    }

    public void EnterShop(shopSign s)
    {
        shopsign = s;
        onEnterShop?.Invoke();
    }

    public void ExitShop()
    {
        shopsign = null;
        onExitShop?.Invoke();
    }

    
    public bool TryBuy(inventoryItem i)
    {
        if (CanBuy(i))
        {
            //addToInventory(i, 1);
            return true;
        }
        return false;
    }

    private bool CanBuy(inventoryItem i)
    {
        //code will be added later
        return true;
    }
}