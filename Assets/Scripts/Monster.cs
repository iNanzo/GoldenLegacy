using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterData monsterData; // ScriptableObject reference
    private int currentHealth;
    private StateMachine stateMachine;
    private Player player;

    void Start()
    {
        player = Object.FindFirstObjectByType<Player>();

        // Ensure HP is set before updating UI
        currentHealth = Random.Range(monsterData.minHP, monsterData.maxHP + 1);

        // If the state machine exists, update the UI immediately
        if (stateMachine != null)
        {
            stateMachine.UpdateMonsterHPText();
        }
    }

    public void SetStateMachine(StateMachine machine)
    {
        stateMachine = machine;
        stateMachine.UpdateMonsterHPText(); // Ensure HP text updates when monster appears
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
        stateMachine.UpdateMonsterHPText(); // Update HP text after taking damage

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
            player.TakeDamage(damageDealt);
            Debug.Log($"{monsterData.monsterName} attacks for {damageDealt} damage!");
        }
    }

    void Die()
    {
        Debug.Log($"{monsterData.monsterName} defeated!");

        // Give gold based on max HP
        int goldReward = monsterData.GetGoldReward();
        player.AddGold(goldReward);
        Debug.Log($"Player received {goldReward} gold!");

        // Drop a random item from the loot table
        if (monsterData.lootTable != null && monsterData.lootTable.Length > 0)
        {
            string droppedItem = monsterData.lootTable[Random.Range(0, monsterData.lootTable.Length)];
            Debug.Log($"{monsterData.monsterName} dropped: {droppedItem}");
            player.AddToInventory(droppedItem);
        }
        else
        {
            Debug.Log($"{monsterData.monsterName} had no loot.");
        }

        player.ResetHealth();
        stateMachine.MonsterDefeated();
        Destroy(gameObject);
    }

    // Get Monster Name for HP Display
    public string GetMonsterName()
    {
        return monsterData.monsterName;
    }

    // Get Current HP for HP Display
    public int GetCurrentHP()
    {
        return currentHealth;
    }
}
