using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class inventoryItem : ScriptableObject
{
    [SerializeField] private string itemName = "Name";
    [SerializeField, Multiline] private string description = "Description";
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int value = 1;

    [SerializeField] private GameObject prefab = null;

    public Sprite Icon => icon;
    public GameObject Prefab => prefab;

    public string ItemName => itemName;
    public string Description => description;

    public void GotItemCheck()
    {
        string player = GameObject.FindGameObjectWithTag("Player").name;

        Debug.Log($"{player} got item");
    }

    public int GetPrice()
    {
        //code will be added later
        return value;
    }
}
