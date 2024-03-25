using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


public class inventory : MonoBehaviour
{
    private Dictionary<Item, int> Inventory = new Dictionary<Item, int>();

    private int maxItemSlots = 10;

    [Header("Your Items")]
    [SerializeField] private bool loadSavedItems = true;
    

    public delegate void OnInventoryChange();
    public OnInventoryChange onInventoryChange;

    private void Start()
    {
        
    }

    IEnumerator DoLateInit()
    {
        yield return new WaitForSeconds(.05f);
        onInventoryChange?.Invoke();
    }

    public void AddToInventory(Item item, int count = 1)
    {
        //
    }

    public void RemoveFromInventory(Item item, int count)
    {
        //
    }

    public void DropItem(Item item, int count)
    {
        //
    }
}
