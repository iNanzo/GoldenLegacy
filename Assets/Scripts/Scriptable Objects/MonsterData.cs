using UnityEngine;

public enum MonsterRarity { Common, Uncommon, Rare, Legendary }
public enum MonsterTrait { None, Berserk, Vampiric, Golden }
public enum MonsterSize { Normal, Large, Small }

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
    public string[] lootTable; // List of possible loot items

    [Header("Trait Settings")]
    public MonsterTrait[] possibleTraits; // Primary traits
    public bool canBeLargeOrSmall = true; // Should this monster be allowed size changes?

    // Gold reward calculation
    public int GetGoldReward(MonsterTrait trait, MonsterSize size)
    {
        int baseGold = maxHP; // Base gold = monster's max HP

        // Apply trait-based modifiers
        if (trait == MonsterTrait.Golden) baseGold = Mathf.RoundToInt(baseGold * 1.5f);
        if (trait == MonsterTrait.Berserk || trait == MonsterTrait.Vampiric) baseGold = Mathf.RoundToInt(baseGold * 1.2f);
        
        // Apply size modifiers
        if (size == MonsterSize.Large) baseGold = Mathf.RoundToInt(baseGold * 1.1f);
        if (size == MonsterSize.Small) baseGold = Mathf.RoundToInt(baseGold * 0.9f);

        return baseGold;
    }
}
