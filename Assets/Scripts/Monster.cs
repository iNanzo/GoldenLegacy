using UnityEngine;
using System.Collections;
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
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        player = Object.FindFirstObjectByType<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // ✅ Get SpriteRenderer for visual effects

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
            if (roll < 0.15f) sizeModifier = MonsterSize.Giant; // ✅ 15% chance for Giant
            else if (roll < 0.30f) sizeModifier = MonsterSize.Large;
            else if (roll > 0.75f && roll < 0.90f) sizeModifier = MonsterSize.Small;
            else if (roll >= 0.90f) sizeModifier = MonsterSize.Tiny; // ✅ 10% chance for Tiny
            else sizeModifier = MonsterSize.Normal;
        }

        ApplySizeModifiers();
        ApplyVisualEffects(); // ✅ Apply effects based on monster traits

        Debug.Log($"{GetMonsterName()} spawned with {currentHealth} HP, {attackPower} Attack, and Trait: {currentTrait}");

        if (stateMachine != null)
        {
            stateMachine.UpdateMonsterHPText();
        }
    }

    void ApplySizeModifiers()
    {
        if (sizeModifier == MonsterSize.Giant)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 1.5f);
            attackPower = Mathf.RoundToInt(attackPower * 1.3f);
            transform.localScale *= 2.5f; // ✅ Massive size
        }
        else if (sizeModifier == MonsterSize.Large)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 1.2f);
            attackPower = Mathf.RoundToInt(attackPower * 1.05f);
            transform.localScale *= 1.5f;
        }
        else if (sizeModifier == MonsterSize.Small)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 0.9f);
            attackPower = Mathf.RoundToInt(attackPower * 0.9f);
            transform.localScale *= 0.5f;
        }
        else if (sizeModifier == MonsterSize.Tiny)
        {
            currentHealth = Mathf.RoundToInt(currentHealth * 0.7f);
            attackPower = Mathf.RoundToInt(attackPower * 0.7f);
            transform.localScale *= 0.25f; // ✅ Tiny size
        }
    }

    void ApplyVisualEffects()
    {
        if (spriteRenderer == null) return;

        if (currentTrait == MonsterTrait.Vampiric)
        {
            spriteRenderer.color = Color.red; // ✅ Red if Vampiric
        }
        else if (currentTrait == MonsterTrait.Golden)
        {
            spriteRenderer.color = Color.yellow; // ✅ Gold if Golden
        }
    }
    void Update()
    {
        if (stateMachine != null && currentHealth > 0)
        {
            stateMachine.UpdateMonsterHPText(); // ✅ Ensure HP updates properly
        }

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
        if (player == null)
        {
            player = FindFirstObjectByType<Player>(); // ✅ Find the player
            if (player == null)
            {
                Debug.LogError("❌ Player not found in scene! Cannot apply attack.");
                return;
            }
        }

        int playerAttack = player.GetAttackPower(); // ✅ Fetch player's attack stat
        Debug.Log($"⚔️ Player attacks for {playerAttack} damage!");
        
        StartCoroutine(FlashWhite()); // ✅ White Flash Effect for attack feedback
        TakeDamage(playerAttack); // ✅ Apply correct damage

        if (currentHealth > 0)
        {
            Retaliate();
        }
    }
    IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;

            // If monster is default white, make it light gray instead
            if (originalColor == Color.white)
            {
                spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f); // Light gray flash for default monsters
            }
            else
            {
                // Brighten existing color
                spriteRenderer.color = new Color(
                    Mathf.Min(originalColor.r + 0.5f, 1f), // Lighten Red
                    Mathf.Min(originalColor.g + 0.5f, 1f), // Lighten Green
                    Mathf.Min(originalColor.b + 0.5f, 1f)  // Lighten Blue
                );
            }

            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = originalColor; // Restore original color
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

        // ✅ Ensure monsterData exists and has a valid name
        string baseName = (monsterData != null && !string.IsNullOrEmpty(monsterData.monsterName)) 
            ? monsterData.monsterName 
            : "Unknown Monster";

        return string.Join(" ", modifiers) + " " + baseName;
    }

    // Returns the monster's current HP for UI display
    public int GetCurrentHP()
    {
        return currentHealth;
    }
}
