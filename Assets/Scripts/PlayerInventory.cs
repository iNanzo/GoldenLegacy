using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public TMP_Text inventoryText;
    public GameObject inventoryPanel;

    private List<MaterialData> inventory = new List<MaterialData>();
    private bool inventoryVisible = false;

    void Start()
    {
        UpdateInventoryUI();
        inventoryPanel.SetActive(false);
    }

    public void AddItem(MaterialData item)
    {
        inventory.Add(item);
        Debug.Log($"Item added: {item.GetMaterialDescription()}");
        UpdateInventoryUI();
    }

    public void ToggleInventory()
    {
        inventoryVisible = !inventoryVisible;
        inventoryPanel.SetActive(inventoryVisible);
        if (inventoryVisible) UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        if (inventoryText != null)
        {
            if (inventory.Count == 0)
            {
                inventoryText.SetText("Inventory:\nEmpty");
            }
            else
            {
                List<string> itemDescriptions = new List<string>();
                foreach (var item in inventory)
                {
                    itemDescriptions.Add(item.GetMaterialDescription());
                }
                inventoryText.SetText("Inventory:\n" + string.Join("\n", itemDescriptions));
            }
        }
    }

    public List<MaterialData> GetInventoryItems()
    {
        return inventory;
    }

    public void RemoveItem(MaterialData item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            UpdateInventoryUI();
            Debug.Log($"Removed {item.GetMaterialDescription()} from inventory.");
        }
        else
        {
            Debug.LogWarning("Tried to remove a material not in inventory.");
        }
    }
}
