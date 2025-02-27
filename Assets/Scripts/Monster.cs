using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterData monsterData; // ScriptableObject reference
    private int currentHealth;
    private int attackPower;
    private MonsterTrait currentTrait; // Primary trait
    private MonsterSize sizeModifier; // Size modifier
    private StateMachine stateMachine;
    private Player player;

    void Start()
    {
        player = Object.FindFirstObjectByType<Player>();

        currentHealth = Random.Range(monsterData.minHP, monsterData.maxHP + 1);
        attackPower = Random.Range(monsterData.minDamage, monsterData.maxDamage + 1);

        // Randomly assign a Primary Trait (50% chance to have no trait)
        if (monsterData.possibleTraits.Length > 0 && Random.value > 0.5f)
        {
            currentTrait = monsterData.possibleTraits[Random.Range(0, monsterData.possibleTraits.Length)];
        }
        else
        {
            currentTrait = MonsterTrait.None;
        }

        // Assign a Size Modifier (50% chance to stay normal)
        if (monsterData.canBeLargeOrSmall)
        {
            float roll = Random.value;
            if (roll < 0.25f) sizeModifier = MonsterSize.Large;
            else if (roll > 0.75f) sizeModifier = MonsterSize.Small;
            else sizeModifier = MonsterSize.Normal;
        }

        // Apply Size Modifiers
        if (sizeModifier == MonsterSize.Large)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 1.2f); // 20% more HP
            attackPower = Mathf.RoundToInt(attackPower * 1.05f); // 5% more damage
        }
        else if (sizeModifier == MonsterSize.Small)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 0.9f); // 10% less HP
            attackPower = Mathf.RoundToInt(attackPower * 0.9f); // 10% less damage
        }

        Debug.Log($"{GetMonsterName()} spawned with {currentHealth} HP, {attackPower} Attack, and Trait: {currentTrait}");

        // 🛠 FIX: Ensure the UI updates after all values are set
        if (stateMachine != null)
        {
            stateMachine.UpdateMonsterHPText();
        }
    }

    public void SetStateMachine(StateMachine machine)
    {
        stateMachine = machine;
        // Ensure UI updates after setting the state machine
        if (stateMachine != null)
        {
            stateMachine.UpdateMonsterHPText();
        }
    }

    void OnMouseDown()
    {
        TakeDamage(1); // Player deals 1 damage

        if (currentHealth > 0) // Monster retaliates if alive
        {
            Retaliate();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Ensure HP text updates correctly
        if (stateMachine != null)
        {
            stateMachine.UpdateMonsterHPText();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Retaliate()
    {
        if (player != null)
        {
            int damageDealt = Random.Range(monsterData.minDamage, monsterData.maxDamage + 1);

            if (currentTrait == MonsterTrait.Berserk)
            {
                damageDealt = Mathf.RoundToInt(damageDealt * 1.5f); // 50% more damage
            }

            if (sizeModifier == MonsterSize.Large)
            {
                damageDealt = Mathf.RoundToInt(damageDealt * 1.05f); // 5% more damage
            }
            else if (sizeModifier == MonsterSize.Small)
            {
                damageDealt = Mathf.RoundToInt(damageDealt * 0.9f); // 10% less damage
            }

            if (currentTrait == MonsterTrait.Vampiric)
            {
                int healAmount = Mathf.RoundToInt(damageDealt * 0.1f); // Heals 10% of damage dealt
                currentHealth += healAmount;
                Debug.Log($"{GetMonsterName()} healed for {healAmount} HP!");
            }

            player.TakeDamage(damageDealt);
            Debug.Log($"{GetMonsterName()} attacks for {damageDealt} damage!");
        }
    }

    void Die()
    {
        Debug.Log($"{GetMonsterName()} defeated!");

        // Calculate gold with trait & size scaling
        int goldReward = monsterData.GetGoldReward(currentTrait, sizeModifier);
        player.AddGold(goldReward);
        Debug.Log($"Player received {goldReward} gold!");

        // Golden monsters drop everything in their loot table
        if (currentTrait == MonsterTrait.Golden)
        {
            Debug.Log($"{GetMonsterName()} dropped ALL loot items!");
            foreach (string item in monsterData.lootTable)
            {
                player.AddToInventory(item);
            }
        }
        else if (monsterData.lootTable.Length > 0)
        {
            string droppedItem = monsterData.lootTable[Random.Range(0, monsterData.lootTable.Length)];
            player.AddToInventory(droppedItem);
            Debug.Log($"{GetMonsterName()} dropped: {droppedItem}");
        }

        player.ResetHealth();
        stateMachine.MonsterDefeated();
        Destroy(gameObject);
    }

    // Returns the monster's formatted name with traits and size
    public string GetMonsterName()
    {
        string prefix = "";

        // Add size modifier if not normal
        if (sizeModifier == MonsterSize.Large) prefix += "Large ";
        else if (sizeModifier == MonsterSize.Small) prefix += "Small ";

        // Add trait if not None
        if (currentTrait == MonsterTrait.Berserk) prefix += "Berserk ";
        else if (currentTrait == MonsterTrait.Vampiric) prefix += "Vampiric ";
        else if (currentTrait == MonsterTrait.Golden) prefix += "Golden ";

        return prefix + monsterData.monsterName; // Final formatted name
    }

    // Returns the monster's current HP for UI display
    public int GetCurrentHP()
    {
        return currentHealth;
    }
}
