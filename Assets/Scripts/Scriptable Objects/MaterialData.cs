using UnityEngine;
using System.Collections.Generic;

public enum MaterialType { Wood, Stone, Iron, Gold, Leather, Bone, Livingwood, Ruby, Sapphire, Diamond, Dragonscale }
public enum MaterialTrait { None, Flawed, Flawless }
public enum MaterialSize { Normal, Small, Big }
public enum ResistanceType { None, Fire, IceWater, Physical, All }
public enum SpecialEffect { None, AgilityBoost, HPRegen, FireDamage, MagicBoost, IncreasedDurability }

[CreateAssetMenu(fileName = "NewMaterial", menuName = "Crafting/Create New Material")]
public class MaterialData : ScriptableObject
{
    public CraftableItemType itemType; // ✅ Stores assigned item type upon crafting
    public MaterialType materialType;

    [Header("Base Stats")]
    public int minAttack;
    public int maxAttack;
    public int hpBonus;
    public int goldBonus;
    public SpecialEffect specialEffect;

    [Header("Randomized Properties")]
    public MaterialTrait materialTrait;
    public MaterialSize materialSize;

    public string GetMaterialDescription()
    {
        List<string> modifiers = new List<string>();

        if (materialSize != MaterialSize.Normal)
            modifiers.Add(materialSize.ToString());

        if (materialTrait != MaterialTrait.None)
            modifiers.Add(materialTrait.ToString());

        modifiers.Add(materialType.ToString());

        return string.Join(" ", modifiers);
    }

    public void AssignRandomTraitAndSize()
    {
        if (Random.value > 0.5f)
        {
            materialTrait = MaterialTrait.None;
        }
        else
        {
            MaterialTrait[] possibleTraits = { MaterialTrait.Flawed, MaterialTrait.Flawless };
            materialTrait = possibleTraits[Random.Range(0, possibleTraits.Length)];
        }

        float roll = Random.value;
        if (roll < 0.25f) materialSize = MaterialSize.Small;
        else if (roll > 0.75f) materialSize = MaterialSize.Big;
        else materialSize = MaterialSize.Normal;
    }

    public int GetModifiedStat(int baseStat)
    {
        int modifiedStat = baseStat;

        if (materialTrait == MaterialTrait.Flawed)
            modifiedStat = Mathf.RoundToInt(baseStat * 0.9f);
        else if (materialTrait == MaterialTrait.Flawless)
            modifiedStat = Mathf.RoundToInt(baseStat * 1.1f);

        if (materialSize == MaterialSize.Small)
            modifiedStat = Mathf.RoundToInt(modifiedStat * 0.9f);
        else if (materialSize == MaterialSize.Big)
            modifiedStat = Mathf.RoundToInt(modifiedStat * 1.1f);

        return modifiedStat;
    }
}
