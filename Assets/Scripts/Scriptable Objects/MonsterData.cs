using UnityEngine;
using System.Collections.Generic;

public enum MonsterRarity { Common, Uncommon, Rare, Legendary }
public enum MonsterTrait { None, Berserk, Vampiric, Golden }
public enum MonsterSize { Normal, Large, Small, Tiny, Giant }

[System.Serializable]
public class LootEntry
{
    public MaterialData material;
    public float dropChance; // Drop weight in percentage (0-100%)
}

[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Create New Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterName = "Monster";
    public MonsterRarity rarity = MonsterRarity.Common; // Now properly included!

    [Header("Health Settings")]
    public int minHP = 5;
    public int maxHP = 10;

    [Header("Attack Settings")]
    public int minDamage = 1;
    public int maxDamage = 3;

    [Header("Loot Settings")]
    public LootEntry[] materialLootTable; // Stores materials with drop rates

    [Header("Trait Settings")]
    public MonsterTrait[] possibleTraits; // Primary traits
    public bool canBeLargeOrSmall = true; // Should this monster be allowed size changes?

    // Gold reward calculation
    public int GetGoldReward(MonsterTrait trait, MonsterSize size)
    {
        float multiplier = 1f;

        // Trait-based multipliers
        Dictionary<MonsterTrait, float> traitMultipliers = new Dictionary<MonsterTrait, float>
        {
            { MonsterTrait.Golden, 1.5f },
            { MonsterTrait.Berserk, 1.2f },
            { MonsterTrait.Vampiric, 1.2f }
        };

        // Size-based multipliers
        Dictionary<MonsterSize, float> sizeMultipliers = new Dictionary<MonsterSize, float>
        {
            { MonsterSize.Giant, 1.5f },
            { MonsterSize.Large, 1.25f },
            { MonsterSize.Small, 0.9f },
            { MonsterSize.Tiny, 0.7f }
        };

        if (traitMultipliers.TryGetValue(trait, out float traitMult))
        {
            multiplier *= traitMult;
        }

        if (sizeMultipliers.TryGetValue(size, out float sizeMult))
        {
            multiplier *= sizeMult;
        }

        return Mathf.RoundToInt(maxHP * multiplier);
    }
}
