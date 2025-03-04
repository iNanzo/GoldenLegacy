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

        string craftedItemDescription = GenerateCraftedItemDescription(recipe);

        if (player != null)
        {
            Debug.Log($"✅ Crafted {recipe.itemType} using: {recipe.GetMaterialLog()}");
            Debug.Log($"- Total Attack: {recipe.GetTotalAttack()}");
            Debug.Log($"- Total HP: {recipe.GetTotalHP()}");
            Debug.Log($"- Total Value: {recipe.GetTotalGoldValue()} gold");

            // Add the crafted item to the player's inventory as a new MaterialData
            player.inventory.AddItem(CreateCraftedItemAsMaterial(recipe, craftedItemDescription));
            Debug.Log($"✅ Added crafted {recipe.itemType} to inventory: {craftedItemDescription}");
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
                return materials.Length == 2; // Crystal is optional
            case CraftableItemType.Helmet:
                return materials.Length == 1; // Crystal is optional
            case CraftableItemType.Armor:
                return materials.Length == 2 && crystal == null; // No crystal allowed
            case CraftableItemType.Ring:
                return materials.Length == 1; // Crystal is optional
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
        craftedMaterial.materialTrait = MaterialTrait.Flawless; // Optional or average of materials
        craftedMaterial.materialSize = MaterialSize.Normal;

        Debug.Log($"✅ Created crafted item as MaterialData: {craftedItemDescription}");
        return craftedMaterial;
    }
}
