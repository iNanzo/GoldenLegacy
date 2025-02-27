using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    public enum GameState { Idle, Spawning, Combat }
    public GameState currentState = GameState.Idle;

    [System.Serializable]
    public class SpawnCategory
    {
        public string name;
        public GameObject[] monsters; // Array of monsters for this category
        public Button spawnButton; // Corresponding button
    }

    public List<SpawnCategory> spawnCategories = new List<SpawnCategory>();
    public Transform spawnPoint;
    private GameObject currentMonster;
    private Player player;

    public TMP_Text monsterHPText; // UI Text for Monster HP

    void Start()
    {
        player = Object.FindFirstObjectByType<Player>(); // Get player reference
        player.SetStateMachine(this); // Connect player to state machine

        foreach (var category in spawnCategories)
        {
            category.spawnButton.onClick.RemoveAllListeners();
            category.spawnButton.onClick.AddListener(() => SpawnRandomMonster(category));
        }

        monsterHPText.gameObject.SetActive(false); // Hide Monster HP text initially
        UpdateState(GameState.Idle);
    }

    void SpawnRandomMonster(SpawnCategory category)
    {
        if (currentState == GameState.Idle && category.monsters.Length > 0)
        {
            GameObject randomMonster = category.monsters[Random.Range(0, category.monsters.Length)];
            currentMonster = Instantiate(randomMonster, spawnPoint.position, Quaternion.identity);
            Monster monsterScript = currentMonster.GetComponent<Monster>();

            if (monsterScript != null)
            {
                monsterScript.SetStateMachine(this); // Connect monster to state machine
                UpdateMonsterHPText(); // Ensure UI updates immediately
            }

            UpdateState(GameState.Combat);
        }
    }

    public void MonsterDefeated()
    {
        if (currentState == GameState.Combat)
        {
            monsterHPText.gameObject.SetActive(false); // Hide HP text when monster dies
            UpdateState(GameState.Idle);
        }
    }

    public void PlayerDied()
    {
        if (currentState == GameState.Combat)
        {
            monsterHPText.gameObject.SetActive(false); // Hide HP text if player dies
            UpdateState(GameState.Idle);
        }
    }

    public void UpdateMonsterHPText()
    {
        if (currentMonster != null)
        {
            Monster monsterScript = currentMonster.GetComponent<Monster>();
            if (monsterScript != null)
            {
                monsterHPText.gameObject.SetActive(true); // Show HP text
                monsterHPText.SetText($"{monsterScript.GetMonsterName()}: {monsterScript.GetCurrentHP()} HP");
            }
        }
    }

    void UpdateState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Idle:
                foreach (var category in spawnCategories)
                {
                    category.spawnButton.gameObject.SetActive(true);
                }
                break;

            case GameState.Combat:
                foreach (var category in spawnCategories)
                {
                    category.spawnButton.gameObject.SetActive(false);
                }
                break;
        }
    }

}
