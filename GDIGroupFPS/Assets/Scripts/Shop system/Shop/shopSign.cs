using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopSign : MonoBehaviour
{
    [SerializeField] inventory inventory = null;

    private shop currentShop = null;

    public shop GetCurrentShop => currentShop;

    public delegate void OnEnterShop(shop shop);
    public delegate void OnExitShop();
    public OnEnterShop onEnterShop;
    public OnExitShop onExitShop;

    private void Start()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            if (other.TryGetComponent(out currentShop))
                EnterShop();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            ExitShop();
        }
    }

    private void EnterShop()
    {
        currentShop.EnterShop(this);

        onEnterShop?.Invoke(currentShop);
    }

    private void ExitShop()
    {
        currentShop.ExitShop();

        currentShop = null;

        onExitShop?.Invoke();
    }

    public bool TryBuyItem(inventoryItem i, int number)
    {
        //code will be added later
        return true;
    }
}