using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Create New Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterName = "Monster";

    [Header("Health Settings")]
    public int minHP = 5;
    public int maxHP = 10;

    [Header("Attack Settings")]
    public int minDamage = 1;
    public int maxDamage = 3;

    [Header("Loot Settings")]
    public string[] lootTable; // List of possible loot items

    // Calculate gold drop based on max HP
    public int GetGoldReward()
    {
        return maxHP; // Example: Gold = max HP * 2
    }
}
