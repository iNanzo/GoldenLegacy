using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    public enum GameState { Idle, Spawning, Combat }
    public GameState currentState = GameState.Idle;
    public SpriteRenderer backgroundRenderer; // ✅ Reference to the Background GameObject's SpriteRenderer

   [System.Serializable]
    public class SpawnCategory
    {
        public string name;
        public GameObject[] monsters; // Array of monsters for this category
        public Button spawnButton; // Corresponding button
        public Sprite backgroundSprite; // ✅ New background sprite
    }
    public List<SpawnCategory> spawnCategories = new List<SpawnCategory>();
    public Transform spawnPoint;
    private GameObject currentMonster;
    private Player player;
    public bool buttonActive = true;
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
            currentMonster = Instantiate(category.monsters[Random.Range(0, category.monsters.Length)], spawnPoint.position, Quaternion.identity);
            Monster monsterScript = currentMonster.GetComponent<Monster>();

            if (monsterScript != null)
            {
                monsterScript.SetStateMachine(this);
                monsterHPText.gameObject.SetActive(true); // ✅ Show HP text

                UpdateMonsterHPText(); // ✅ Ensure HP updates immediately
            }

            // ✅ Change the background sprite when a spawn button is clicked
            if (backgroundRenderer != null && category.backgroundSprite != null)
            {
                backgroundRenderer.sprite = category.backgroundSprite;
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
        if (currentMonster == null)
        {
            monsterHPText.gameObject.SetActive(false); // Hide if no monster
            return;
        }

        Monster monsterScript = currentMonster.GetComponent<Monster>();
        if (monsterScript != null)
        {
            string monsterName = monsterScript.GetMonsterName();
            int monsterHP = monsterScript.GetCurrentHP();

            monsterHPText.gameObject.SetActive(true); // ✅ Ensure UI is visible
            monsterHPText.SetText($"{monsterName}: {monsterHP} HP");
        }
        else
        {
            monsterHPText.gameObject.SetActive(false); // ✅ Hide if monster is null
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
                    buttonActive = true;
                }
                break;

            case GameState.Combat:
                foreach (var category in spawnCategories)
                {
                    category.spawnButton.gameObject.SetActive(false);
                    buttonActive = false;
                }
                break;
        }
    }

    public bool zoneActive()
    {
        return buttonActive;
    }
}
