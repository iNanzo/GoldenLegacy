using UnityEngine;
using TMPro; // Use TextMeshPro

public class Monster : MonoBehaviour
{
    public string monsterName = "Monster"; // Default name
    public int maxHealth = 5;
    private int currentHealth;
    private StateMachine stateMachine;
    private Player player;

    public int damage = 1;
    public int goldReward = 10;
    public TMP_Text monsterHPText; // UI Text for Monster HP

    void Start()
    {
        currentHealth = maxHealth;
        player = Object.FindFirstObjectByType<Player>();

        // Auto-find the HP Text in the scene (must be named "MonsterHPText")
        monsterHPText = Object.FindFirstObjectByType<TMP_Text>();

        if (monsterHPText == null)
        {
            Debug.LogError("Monster HP Text not found! Make sure it's named 'MonsterHPText' in the Canvas.");
        }
        else
        {
            UpdateMonsterHPText();
        }
    }

    public void SetStateMachine(StateMachine machine)
    {
        stateMachine = machine;
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
        UpdateMonsterHPText();
        Debug.Log($"{monsterName} took {damage} damage! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Retaliate()
    {
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    void Die()
    {
        Debug.Log($"{monsterName} defeated!");
        player.AddGold(goldReward);
        player.ResetHealth();
        stateMachine.MonsterDefeated();

        // Clear the Monster HP Text when the monster dies
        if (monsterHPText != null)
        {
            monsterHPText.SetText("");
        }

        Destroy(gameObject);
    }

    void UpdateMonsterHPText()
    {
        if (monsterHPText != null)
        {
            monsterHPText.SetText($"{monsterName}: {currentHealth} HP");
        }
    }
}
