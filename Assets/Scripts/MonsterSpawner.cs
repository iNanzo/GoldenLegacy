using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public Transform spawnPoint;
    public MonsterData[] monsterPool;

    private Dictionary<MonsterRarity, int> rarityWeights = new Dictionary<MonsterRarity, int>()
    {
        { MonsterRarity.Common, 60 },    
        { MonsterRarity.Uncommon, 25 },  
        { MonsterRarity.Rare, 10 },      
        { MonsterRarity.Legendary, 5 }   
    };

    public void SpawnMonster()
    {
        if (monsterPool.Length == 0)
        {
            Debug.LogError("No monsters assigned to this zone!");
            return;
        }

        MonsterData selectedMonster = GetRandomMonsterByRarity();
        if (selectedMonster == null) return;

        GameObject newMonster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
        Monster monsterScript = newMonster.GetComponent<Monster>();

        if (monsterScript != null)
        {
            monsterScript.monsterData = selectedMonster;
        }
    }

    private MonsterData GetRandomMonsterByRarity()
    {
        List<MonsterData> weightedMonsters = new List<MonsterData>();

        foreach (MonsterData monster in monsterPool)
        {
            if (rarityWeights.TryGetValue(monster.rarity, out int weight))
            {
                for (int i = 0; i < weight; i++) weightedMonsters.Add(monster);
            }
        }

        return weightedMonsters.Count > 0 ? weightedMonsters[Random.Range(0, weightedMonsters.Count)] : null;
    }
}
