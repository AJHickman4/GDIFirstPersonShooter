using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class slotHolderUI<T> : MonoBehaviour
{
    protected Dictionary<T, int> inventoryOfType;

    [SerializeField] private GameObject slotUI = null;
    [SerializeField] private GameObject holderObject = null;
    [SerializeField] private Transform contentParent = null;

    protected virtual void InitializeUI(Dictionary<T, int> inventory, int numSlots)
    {
        inventoryOfType = inventory;

        CreateItemSlots(numSlots);
    }

    private void CreateItemSlots(int numSlotsToCreate)
    {
        
    }
}