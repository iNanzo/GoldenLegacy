using UnityEngine;
using UnityEngine.UI;
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

    void Start()
    {
        player = Object.FindFirstObjectByType<Player>(); // Get player reference
        player.SetStateMachine(this); // Connect player to state machine

        foreach (var category in spawnCategories)
        {
            category.spawnButton.onClick.RemoveAllListeners();
            category.spawnButton.onClick.AddListener(() => SpawnRandomMonster(category));
        }
        UpdateState(GameState.Idle);
    }

    void SpawnRandomMonster(SpawnCategory category)
    {
        if (currentState == GameState.Idle && category.monsters.Length > 0)
        {
            GameObject randomMonster = category.monsters[Random.Range(0, category.monsters.Length)];
            currentMonster = Instantiate(randomMonster, spawnPoint.position, Quaternion.identity);
            currentMonster.GetComponent<Monster>().SetStateMachine(this);

            UpdateState(GameState.Combat);
        }
    }

    public void MonsterDefeated()
    {
        if (currentState == GameState.Combat)
        {
            UpdateState(GameState.Idle);
        }
    }

    public void PlayerDied()
    {
        if (currentState == GameState.Combat)
        {
            UpdateState(GameState.Idle);
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
