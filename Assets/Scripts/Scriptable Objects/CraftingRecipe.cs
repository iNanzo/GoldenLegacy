using UnityEngine;
using System.Collections.Generic;

public enum CraftableItemType { Sword, Helmet, Armor, Ring }

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Create New Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public CraftableItemType itemType;

    [Header("Required Materials")]
    public MaterialData[] requiredMaterials; // Array for materials needed
    public MaterialData crystalSlot; // Optional

    private List<string> materialLog = new List<string>();

    public int GetTotalAttack()
    {
        int attack = 0;
        foreach (var material in requiredMaterials)
        {
            attack += material.GetModifiedStat(material.minAttack);
        }
        if (crystalSlot) attack += crystalSlot.GetModifiedStat(crystalSlot.minAttack);
        return attack;
    }

    public int GetTotalHP()
    {
        int hp = 0;
        foreach (var material in requiredMaterials)
        {
            hp += material.GetModifiedStat(material.hpBonus);
        }
        if (crystalSlot) hp += crystalSlot.GetModifiedStat(crystalSlot.hpBonus);
        return hp;
    }

    public int GetTotalGoldValue()
    {
        int gold = 0;
        foreach (var material in requiredMaterials)
        {
            gold += material.GetModifiedStat(material.goldBonus);
        }
        if (crystalSlot) gold += crystalSlot.GetModifiedStat(crystalSlot.goldBonus);
        return gold;
    }

    public void LogCraftedMaterials()
    {
        materialLog.Clear();
        foreach (var material in requiredMaterials)
        {
            materialLog.Add(material.GetMaterialDescription());
        }
        if (crystalSlot) materialLog.Add(crystalSlot.GetMaterialDescription());
    }

    public string GetMaterialLog()
    {
        return string.Join(" + ", materialLog);
    }
}
