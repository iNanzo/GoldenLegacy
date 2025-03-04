using UnityEngine;
using System.Collections.Generic;

public class Monster : MonoBehaviour
{
    public MonsterData monsterData;
    private int currentHealth;
    private int attackPower;
    private MonsterTrait currentTrait;
    private MonsterSize sizeModifier;
    private StateMachine stateMachine;
    private Player player;

    void Start()
    {
        player = Object.FindFirstObjectByType<Player>();

        currentHealth = Random.Range(monsterData.minHP, monsterData.maxHP + 1);
        attackPower = Random.Range(monsterData.minDamage, monsterData.maxDamage + 1);

        if (monsterData.possibleTraits.Length > 0 && Random.value > 0.5f)
        {
            currentTrait = monsterData.possibleTraits[Random.Range(0, monsterData.possibleTraits.Length)];
        }
        else
        {
            currentTrait = MonsterTrait.None;
        }

        if (monsterData.canBeLargeOrSmall)
        {
            float roll = Random.value;
            if (roll < 0.25f) sizeModifier = MonsterSize.Large;
            else if (roll > 0.75f) sizeModifier = MonsterSize.Small;
            else sizeModifier = MonsterSize.Normal;
        }

        ApplySizeModifiers();

        Debug.Log($"{GetMonsterName()} spawned with {currentHealth} HP, {attackPower} Attack, and Trait: {currentTrait}");

        if (stateMachine != null)
        {
            stateMachine.UpdateMonsterHPText();
        }
    }

    void ApplySizeModifiers()
    {
        if (sizeModifier == MonsterSize.Large)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 1.2f);
            attackPower = Mathf.RoundToInt(attackPower * 1.05f);
        }
        else if (sizeModifier == MonsterSize.Small)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 0.9f);
            attackPower = Mathf.RoundToInt(attackPower * 0.9f);
        }
    }

    void Update()
    {
        if (stateMachine.zoneActive())
        {
            Debug.Log("Destroying monster due to player death.");
            stateMachine.MonsterDefeated();
            Destroy(gameObject);
        }
    }

    public void SetStateMachine(StateMachine machine)
    {
        stateMachine = machine;
        stateMachine?.UpdateMonsterHPText();
    }

    void OnMouseDown()
    {
        TakeDamage(1);
        if (currentHealth > 0)
        {
            Retaliate();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        stateMachine?.UpdateMonsterHPText();

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
            if (currentTrait == MonsterTrait.Berserk) damageDealt = Mathf.RoundToInt(damageDealt * 1.5f);
            if (sizeModifier == MonsterSize.Large) damageDealt = Mathf.RoundToInt(damageDealt * 1.05f);
            else if (sizeModifier == MonsterSize.Small) damageDealt = Mathf.RoundToInt(damageDealt * 0.9f);

            if (currentTrait == MonsterTrait.Vampiric)
            {
                int healAmount = Mathf.RoundToInt(damageDealt * 0.1f);
                currentHealth += healAmount;
                Debug.Log($"{GetMonsterName()} healed for {healAmount} HP!");
            }

            player.TakeDamage(damageDealt);
            Debug.Log($"{GetMonsterName()} attacks for {damageDealt} damage!");

            // ✅ Instead of calling Destroy here, we set a flag for Update() to handle it
            if (player.GetCurrentHP() <= 0)
            {
                Debug.Log("Player died! Monster will despawn when zones reappear.");
            }
        }
    }

    void Die()
    {
        Debug.Log($"{GetMonsterName()} defeated!");

        // Calculate gold with trait & size scaling
        int goldReward = monsterData.GetGoldReward(currentTrait, sizeModifier);
        player.AddGold(goldReward);
        Debug.Log($"Player received {goldReward} gold!");

        foreach (var lootEntry in monsterData.materialLootTable)
        {
            if (currentTrait == MonsterTrait.Golden || Random.value * 100 <= lootEntry.dropChance)
            {
                MaterialData droppedItem = Instantiate(lootEntry.material);
                droppedItem.AssignRandomTraitAndSize();
                player.AddToInventory(droppedItem);
                Debug.Log($"{GetMonsterName()} dropped: {droppedItem.GetMaterialDescription()}");
            }
        }

        player.ResetHealth();
        stateMachine.MonsterDefeated();
        Destroy(gameObject);
    }

    // Returns the monster's formatted name with traits and size
    public string GetMonsterName()
    {
        List<string> modifiers = new List<string>();
        if (sizeModifier != MonsterSize.Normal) modifiers.Add(sizeModifier.ToString());
        if (currentTrait != MonsterTrait.None) modifiers.Add(currentTrait.ToString());
        return string.Join(" ", modifiers) + " " + monsterData.monsterName;
    }


    // Returns the monster's current HP for UI display
    public int GetCurrentHP()
    {
        return currentHealth;
    }
}
