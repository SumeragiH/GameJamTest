using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 怪物系统，负责管理怪物的生成、行为和交互等逻辑
/// </summary>
class MonsterSystem : SingletonBaseWithMono<MonsterSystem>
{
    [field: SerializeField] public List<MonsterSpawnerView> monsterPrefabs { get; private set; } = new List<MonsterSpawnerView>(); // 怪物预制体列表，可以在编辑器中设置
    private List<MonsterSpawnerView> monsters = new List<MonsterSpawnerView>();


    void Start()
    {
        EventCenter.Instance.AddListener<MonsterSpawnerView>("怪物死亡", OnMonsterDeath);
    }

    public List<MonsterSpawnerView> GetMonstersPrefab()
    {
        return monsterPrefabs;
    }

    public void AddMonster(MonsterSpawnerView monster, PlotView plotView)
    {
        monsters.Add(monster);
    }

    public void OnMonsterDeath(MonsterSpawnerView monster)
    {
        // TODO: 可能的游戏逻辑？
        Debug.Log("怪物死亡: " + monster.monsterName);
        monsters.Remove(monster);
        Destroy(monster.gameObject);
    }
}