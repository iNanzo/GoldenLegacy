using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // Assign the monster prefab in the Inspector
    public Transform spawnPoint; // Set a spawn location in the Inspector (optional)
    public Button spawnButton; // Assign the UI Button in the Inspector

    void Start()
    {
        // Ensure the button is assigned and add a listener to the onClick event
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnMonster);
        }
    }

    void SpawnMonster()
    {
        if (monsterPrefab != null)
        {
            // Instantiate at a defined spawn point or at the spawner’s position
            Instantiate(monsterPrefab, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Monster Prefab is not assigned!");
        }
    }
}
