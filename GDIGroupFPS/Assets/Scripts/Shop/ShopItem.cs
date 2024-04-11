using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public string itemName;
    public int cost;
    public Sprite icon;
    public string description;

    public ShopItem(string name, int itemCost, Sprite itemIcon, string desc)
    {
        itemName = name;
        cost = itemCost;
        icon = itemIcon;
        description = desc;
    }
}
