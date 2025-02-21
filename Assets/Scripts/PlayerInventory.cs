using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public TMP_Text inventoryText; // Assign in Inspector
    public GameObject inventoryPanel; // Panel that holds inventory UI

    private List<string> inventory = new List<string>();
    private bool inventoryVisible = false;

    void Start()
    {
        UpdateInventoryUI();
        inventoryPanel.SetActive(false); // Hide inventory at start
    }

    public void AddItem(string item)
    {
        inventory.Add(item);
        Debug.Log($"Item added: {item}");
        UpdateInventoryUI();
    }

    public void ToggleInventory()
    {
        inventoryVisible = !inventoryVisible;
        inventoryPanel.SetActive(inventoryVisible);
        if (inventoryVisible) UpdateInventoryUI(); // Ensure UI updates when opened
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
                inventoryText.SetText("Inventory:\n" + string.Join("\n", inventory));
            }
        }
    }
}
