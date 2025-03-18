using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public Player player; 

    public void CraftItem(CraftingRecipe recipe, MaterialData[] materials, MaterialData crystal = null)
    {
        if (!ValidateMaterials(recipe.itemType, materials, crystal))
        {
            Debug.LogError("❌ Invalid materials provided for crafting " + recipe.itemType);
            return;
        }

        // Apply random traits and sizes to each material
        foreach (var material in materials)
        {
            material.AssignRandomTraitAndSize();
        }
        if (crystal != null)
        {
            crystal.AssignRandomTraitAndSize();
        }

        // Assign materials to the recipe for stat calculations
        recipe.requiredMaterials = materials;
        recipe.crystalSlot = crystal;
        recipe.LogCraftedMaterials();

        MaterialData craftedItem = CreateCraftedItem(recipe);

        if (craftedItem == null)
        {
            Debug.LogError("❌ Crafted item is NULL! Something went wrong in CreateCraftedItem()");
            return;
        }

        if (player != null)
        {
            player.inventory.AddCraftedItem(craftedItem);
            Debug.Log($"✅ Crafted item added: {craftedItem.name}");
            LogCraftedItems();
        }
        else
        {
            Debug.LogError("❌ Player reference is missing in CraftingSystem!");
        }
    }

    private bool ValidateMaterials(CraftableItemType itemType, MaterialData[] materials, MaterialData crystal)
    {
        switch (itemType)
        {
            case CraftableItemType.Sword:
                return materials.Length == 2;
            case CraftableItemType.Helmet:
                return materials.Length == 1;
            case CraftableItemType.Armor:
                return materials.Length == 2 && crystal == null;
            case CraftableItemType.Ring:
                return materials.Length == 1;
            default:
                return false;
        }
    }

    private MaterialData CreateCraftedItem(CraftingRecipe recipe)
    {
        MaterialData craftedItem = ScriptableObject.CreateInstance<MaterialData>();

        if (craftedItem == null)
        {
            Debug.LogError("❌ CreateCraftedItem() FAILED - craftedItem is NULL!");
            return null;
        }

        craftedItem.name = recipe.itemType.ToString();
        craftedItem.materialType = MaterialType.Gold;
        craftedItem.minAttack = recipe.GetTotalAttack();
        craftedItem.hpBonus = recipe.GetTotalHP();
        craftedItem.goldBonus = recipe.GetTotalGoldValue();
        craftedItem.materialTrait = MaterialTrait.Flawless;
        craftedItem.materialSize = MaterialSize.Normal;

        Debug.Log($"🛠 Created crafted item: {craftedItem.name}");
        return craftedItem;
    }

    private void LogCraftedItems()
    {
        List<MaterialData> craftedItems = player.inventory.GetCraftedItems();
        if (craftedItems.Count == 0)
        {
            Debug.Log("⚠️ No crafted items stored in inventory!");
        }
        else
        {
            foreach (var item in craftedItems)
            {
                Debug.Log($"📦 {item.name} | ATK: {item.minAttack}, HP: {item.hpBonus}, Value: {item.goldBonus}g");
            }
        }
    }
}
