using UnityEngine;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public void CraftItem(CraftingRecipe recipe, MaterialData[] materials, MaterialData crystal = null)
    {
        // Ensure the number of provided materials matches the recipe requirement
        if (materials.Length != recipe.requiredMaterials.Length)
        {
            Debug.LogError("Incorrect number of materials provided for crafting!");
            return;
        }

        // Assign random traits & sizes before crafting
        foreach (var material in materials)
        {
            material.AssignRandomTraitAndSize();
        }
        if (crystal) crystal.AssignRandomTraitAndSize();

        // Assign materials to the recipe
        recipe.requiredMaterials = materials;
        recipe.crystalSlot = crystal;

        // Log the materials used
        recipe.LogCraftedMaterials();

        Debug.Log($"Crafted {recipe.itemType} using: {recipe.GetMaterialLog()}");
        Debug.Log($"- Attack: {recipe.GetTotalAttack()}");
        Debug.Log($"- HP: {recipe.GetTotalHP()}");
        Debug.Log($"- Value: {recipe.GetTotalGoldValue()} gold");
    }
}
