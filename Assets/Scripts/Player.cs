using UnityEngine;
using TMPro; // Use TextMeshPro

public class Player : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    public int gold = 0;

    public TMP_Text healthText;
    public TMP_Text goldText;

    private StateMachine stateMachine;

    void Start()
    {
        currentHealth = maxHealth;
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
        stateMachine.PlayerDied();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
        Debug.Log($"Player received {amount} gold! Total Gold: {gold}");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthText != null) healthText.SetText($"HP: {currentHealth}");
        if (goldText != null) goldText.SetText($"Gold: {gold}");
    }
}
