using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public TMP_Text inventoryText;
    public GameObject inventoryPanel;

    private List<MaterialData> inventory = new List<MaterialData>();
    private bool inventoryVisible = false;
    private List<MaterialData> craftedItems = new List<MaterialData>();

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

    // ✅ New methods for handling crafted items
    public void AddCraftedItem(MaterialData item)
    {
        if (item == null)
        {
            Debug.LogError("❌ Tried to add NULL crafted item!");
            return;
        }

        craftedItems.Add(item);
        Debug.Log($"✅ Successfully added crafted item: {item.name}");

        // ✅ Debug List Contents
        Debug.Log($"📦 Crafted Items in Inventory ({craftedItems.Count}):");
        foreach (var crafted in craftedItems)
        {
            Debug.Log($"- {crafted.name} | ATK: {crafted.minAttack}, HP: {crafted.hpBonus}, Value: {crafted.goldBonus}g");
        }
    }

    public List<MaterialData> GetCraftedItems()
    {
        if (craftedItems.Count == 0)
        {
            Debug.Log("⚠️ No crafted items available.");
        }
        return craftedItems;
    }

    public void RemoveCraftedItem(MaterialData item)
    {
        if (craftedItems.Contains(item))
        {
            craftedItems.Remove(item);
            Debug.Log($"✅ Removed crafted item: {item.name}");
        }
        else
        {
            Debug.LogWarning("❌ Tried to remove a crafted item that isn't in the inventory.");
        }
    }
}
