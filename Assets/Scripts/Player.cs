using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int baseAttack = 5; // Default attack value
    public int baseMaxHealth = 10; // Default max HP
    public int baseGoldMultiplier = 1; // Default gold multiplier

    private int attack;
    private int maxHealth;
    private int goldMultiplier;

    private int currentHealth;
    public int gold = 0;

    public TMP_Text healthText;
    public TMP_Text goldText;
    public PlayerInventory inventory;

    private StateMachine stateMachine;

    void Start()
    {
        currentHealth = baseMaxHealth;
        attack = baseAttack;
        maxHealth = baseMaxHealth;
        goldMultiplier = baseGoldMultiplier;
        UpdateUI();
    }

    public void SetStateMachine(StateMachine machine)
    {
        stateMachine = machine;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateUI();

        Debug.Log($"Player took {damage} damage! HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        ResetHealth();
        if (stateMachine != null)
        {
            stateMachine.PlayerDied();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount * goldMultiplier; // ✅ Gold multiplier applies
        UpdateUI();
        Debug.Log($"Player received {amount} gold! Total Gold: {gold}");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }
    public void SetStats(int extraAttack, int extraHP, int extraGold)
    {
        attack = baseAttack + extraAttack;
        maxHealth = baseMaxHealth + extraHP;
        goldMultiplier = baseGoldMultiplier + extraGold;

        Debug.Log($"💪 Player Stats Applied - ATK: {attack}, Max HP: {maxHealth}, Gold Multiplier: {goldMultiplier}");

        // ✅ Make sure the player’s HP updates in case of max HP changes
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.SetText($"HP: {currentHealth} / {maxHealth}");

        if (goldText != null)
            goldText.SetText($"Gold: {gold}");
    }

    public void AddToInventory(MaterialData item)
    {
        if (inventory != null)
        {
            inventory.AddItem(item);
            Debug.Log($"Added {item.GetMaterialDescription()} to inventory!");
        }
        else
        {
            Debug.LogError("Inventory reference is missing! Assign it in the Inspector.");
        }
    }

    public int GetCurrentHP()
    {
        return currentHealth;
    }
}
