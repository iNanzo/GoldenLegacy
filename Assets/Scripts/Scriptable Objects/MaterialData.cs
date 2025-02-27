using UnityEngine;

public enum MaterialType { Wood, Stone, Iron, Gold, Leather, Bone, Livingwood, Ruby, Sapphire, Diamond, Dragonscale }
public enum MaterialTrait { None, Flawed, Flawless }
public enum MaterialSize { Normal, Small, Big }
public enum ResistanceType { None, Fire, IceWater, Physical, All }
public enum SpecialEffect { None, AgilityBoost, HPRegen, FireDamage, MagicBoost, IncreasedDurability }

[CreateAssetMenu(fileName = "NewMaterial", menuName = "Crafting/Create New Material")]
public class MaterialData : ScriptableObject
{
    public MaterialType materialType;

    [Header("Base Stats")]
    public int minAttack;
    public int maxAttack;
    public int hpBonus;
    public int goldBonus;
    public ResistanceType resistance;
    public SpecialEffect specialEffect;

    [Header("Randomized Properties")]
    public MaterialTrait materialTrait;
    public MaterialSize materialSize;

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

    public string GetMaterialDescription()
    {
        string description = "";

        if (materialSize == MaterialSize.Big) description += "Big ";
        else if (materialSize == MaterialSize.Small) description += "Small ";

        if (materialTrait == MaterialTrait.Flawed) description += "Flawed ";
        else if (materialTrait == MaterialTrait.Flawless) description += "Flawless ";

        return description + materialType.ToString();
    }
}
