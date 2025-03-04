using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public Player player; // Reference to the player

    public void CraftItem(CraftingRecipe recipe, MaterialData[] materials, MaterialData crystal = null)
    {
        if (!ValidateMaterials(recipe.itemType, materials, crystal))
        {
            Debug.LogError("Invalid materials provided for crafting " + recipe.itemType);
            return;
        }

        // Assign random traits & sizes before crafting
        foreach (var material in materials)
        {
            material.AssignRandomTraitAndSize();
        }
        if (crystal != null) crystal.AssignRandomTraitAndSize();

        recipe.requiredMaterials = materials;
        recipe.crystalSlot = crystal;
        recipe.LogCraftedMaterials();

        string craftedItem = GenerateCraftedItemDescription(recipe);

        if (player != null)
        {
            Debug.Log($"Crafted {recipe.itemType} using: {recipe.GetMaterialLog()}");
            Debug.Log($"- Attack: {recipe.GetTotalAttack()}");
            Debug.Log($"- HP: {recipe.GetTotalHP()}");
            Debug.Log($"- Value: {recipe.GetTotalGoldValue()} gold");

            // Optionally, you could add the crafted item itself to inventory as a string
            player.inventory.AddItem(CreateCraftedItemAsMaterial(recipe, craftedItem));
            Debug.Log($"Added crafted {recipe.itemType} to inventory: {craftedItem}");
        }
        else
        {
            Debug.LogError("Player reference is missing in CraftingSystem!");
        }
    }

    private bool ValidateMaterials(CraftableItemType itemType, MaterialData[] materials, MaterialData crystal)
    {
        switch (itemType)
        {
            case CraftableItemType.Sword:
                if (materials.Length != 2) return false;
                return true; // Crystal is optional

            case CraftableItemType.Helmet:
                if (materials.Length != 1) return false;
                return true; // Crystal is optional

            case CraftableItemType.Armor:
                if (materials.Length != 2) return false;
                if (crystal != null) return false; // No crystal allowed
                return true;

            case CraftableItemType.Ring:
                if (materials.Length != 1) return false;
                return true; // Crystal is optional

            default:
                return false;
        }
    }

    private string GenerateCraftedItemDescription(CraftingRecipe recipe)
    {
        string itemName = recipe.itemType.ToString();
        int totalAttack = recipe.GetTotalAttack();
        int totalHP = recipe.GetTotalHP();
        int totalValue = recipe.GetTotalGoldValue();
        string materialsUsed = recipe.GetMaterialLog();

        return $"{itemName} (ATK: {totalAttack}, HP: {totalHP}, Value: {totalValue}g) - Materials: {materialsUsed}";
    }

    // Optional: Create a placeholder MaterialData to represent the crafted item in inventory
    private MaterialData CreateCraftedItemAsMaterial(CraftingRecipe recipe, string craftedItemDescription)
    {
        MaterialData craftedMaterial = ScriptableObject.CreateInstance<MaterialData>();
        craftedMaterial.materialType = MaterialType.Gold; // Placeholder type
        craftedMaterial.minAttack = recipe.GetTotalAttack();
        craftedMaterial.hpBonus = recipe.GetTotalHP();
        craftedMaterial.goldBonus = recipe.GetTotalGoldValue();
        craftedMaterial.materialTrait = MaterialTrait.Flawless; // Optional
        craftedMaterial.materialSize = MaterialSize.Normal;

        Debug.Log($"Crafted item created as MaterialData: {craftedItemDescription}");
        return craftedMaterial;
    }
}
