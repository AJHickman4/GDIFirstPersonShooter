using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;



[CreateAssetMenu(fileName = "ShopMenu", menuName = "Sciptable Objects/NewShop Item", order = 1)]

public class ShopItem : ScriptableObject
{
    public string itemName;
    public int cost;
    public Sprite icon;
    public string description;
    public ItemType itemType;  
    public float effectValue;   

    public enum ItemType
    {
        HealthUpgrade,
        AmmoCapacityUpgrade,
        DamageUpgrade,
        JumpUpgrade,
        SpeedUpgrade,
        StaminaUpgrade,
        PortalActivation,
        sprintUpgrade,
        RefillaAmmo,
        teleport,
    }
}
