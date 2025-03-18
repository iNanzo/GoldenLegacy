using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    public TMP_Text swordSlotText;
    public TMP_Text helmetSlotText;
    public TMP_Text armorSlotText;
    public TMP_Text ringSlotText;
    public GameObject equipmentPanel;
    public GameObject equipmentListPanel;
    public GameObject equipmentButtonPrefab;

    private Dictionary<CraftableItemType, MaterialData> equippedItems = new Dictionary<CraftableItemType, MaterialData>();
    public Player player; // Add this to store the player reference

    void Start()
    {
        player = FindFirstObjectByType<Player>(); // ✅ Find the player in the scene
        if (player == null)
        {
            Debug.LogError("❌ Player reference is missing in EquipmentManager!");
        }
    }

    public void OpenEquipmentPanel()
    {
        equipmentPanel.SetActive(true);
        RefreshEquipmentList();
    }

    public void CloseEquipmentPanel()
    {
        equipmentPanel.SetActive(false);
    }
    public void EquipItem(MaterialData item)
    {
        CraftableItemType itemType = GetItemType(item);

        // ✅ If item is already equipped in the same slot, unequip it instead
        if (equippedItems.ContainsKey(itemType) && equippedItems[itemType] == item)
        {
            UnequipItem(itemType);
            return;
        }

        // ✅ Ensure the player can equip one of EACH item type
        equippedItems[itemType] = item;
        UpdatePlayerStats();
        UpdateEquipmentUI();
        Debug.Log($"✅ Equipped {item.name} as {itemType}");
    }

    public void UnequipItem(CraftableItemType itemType)
    {
        if (equippedItems.ContainsKey(itemType))
        {
            Debug.Log($"❌ Unequipped {equippedItems[itemType].name}");
            equippedItems.Remove(itemType);
            UpdatePlayerStats();
            UpdateEquipmentUI();
        }
    }

    void UpdatePlayerStats()
    {
        if (player == null)
        {
            Debug.LogError("❌ Player reference is missing in EquipmentManager!");
            return;
        }

        int totalAttack = 0;
        int totalHP = 0;
        int totalGoldBonus = 0;

        // ✅ Add up stats from equipped items
        foreach (var item in equippedItems.Values)
        {
            totalAttack += item.minAttack;
            totalHP += item.hpBonus;
            //totalGoldBonus += item.goldBonus;
        }

        Debug.Log($"🛠 Calculated Player Stats - ATK: {totalAttack}, HP: {totalHP}, Gold Bonus: {totalGoldBonus}");

        player.SetStats(totalAttack, totalHP, totalGoldBonus);
    }


    void UpdateEquipmentUI()
    {
        swordSlotText.text = equippedItems.ContainsKey(CraftableItemType.Sword) ? 
            $"{equippedItems[CraftableItemType.Sword].name}\nATK: {equippedItems[CraftableItemType.Sword].minAttack}\nHP: {equippedItems[CraftableItemType.Sword].hpBonus}\nGold: {equippedItems[CraftableItemType.Sword].goldBonus}g" : "N/A";

        helmetSlotText.text = equippedItems.ContainsKey(CraftableItemType.Helmet) ? 
            $"{equippedItems[CraftableItemType.Helmet].name}\nATK: {equippedItems[CraftableItemType.Helmet].minAttack}\nHP: {equippedItems[CraftableItemType.Helmet].hpBonus}\nGold: {equippedItems[CraftableItemType.Helmet].goldBonus}g" : "N/A";

        armorSlotText.text = equippedItems.ContainsKey(CraftableItemType.Armor) ? 
            $"{equippedItems[CraftableItemType.Armor].name}\nATK: {equippedItems[CraftableItemType.Armor].minAttack}\nHP: {equippedItems[CraftableItemType.Armor].hpBonus}\nGold: {equippedItems[CraftableItemType.Armor].goldBonus}g" : "N/A";

        ringSlotText.text = equippedItems.ContainsKey(CraftableItemType.Ring) ? 
            $"{equippedItems[CraftableItemType.Ring].name}\nATK: {equippedItems[CraftableItemType.Ring].minAttack}\nHP: {equippedItems[CraftableItemType.Ring].hpBonus}\nGold: {equippedItems[CraftableItemType.Ring].goldBonus}g" : "N/A";
    }

    void RefreshEquipmentList()
    {
        Debug.Log("🔍 Refreshing Equipment List...");

        foreach (Transform child in equipmentListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<MaterialData> craftedItems = FindFirstObjectByType<PlayerInventory>().GetCraftedItems();

        // 🔍 Debugging Log: Check how many crafted items exist
        Debug.Log($"📦 Found {craftedItems.Count} crafted items in inventory.");

        if (craftedItems.Count == 0)
        {
            Debug.Log("⚠️ No crafted items found.");
        }

        foreach (var item in craftedItems)
        {
            Debug.Log($"🛠 Crafted Item: {item.name} | ATK: {item.minAttack}, HP: {item.hpBonus}");

            GameObject buttonObj = Instantiate(equipmentButtonPrefab, equipmentListPanel.transform);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = $"{item.name}\nATK: {item.minAttack}, HP: {item.hpBonus}";

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => EquipItem(item));
        }
    }

    bool IsEquipable(MaterialData item)
    {
        return item != null && (item.materialType == MaterialType.Iron || item.materialType == MaterialType.Gold ||
                                item.materialType == MaterialType.Dragonscale || item.materialType == MaterialType.Leather);
    }

    CraftableItemType GetItemType(MaterialData item)
    {
        return item.itemType; // ✅ Return the item type that was assigned during crafting
    }
}
