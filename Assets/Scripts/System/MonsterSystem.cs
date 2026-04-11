using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 怪物系统，负责管理怪物的生成、行为和交互等逻辑
/// </summary>
class MonsterSystem : SingletonBaseWithMono<MonsterSystem>
{
    [field: SerializeField] public List<MonsterView> monsterPrefabs { get; private set; } = new List<MonsterView>(); // 怪物预制体列表，可以在编辑器中设置
    private List<MonsterView> monsters = new List<MonsterView>();


    void Start()
    {
        EventCenter.Instance.AddListener<MonsterView>("怪物死亡", OnMonsterDeath);
    }

    public List<MonsterView> GetMonstersPrefab()
    {
        return monsterPrefabs;
    }

    public void AddMonster(MonsterView monster)
    {
        monsters.Add(monster);
    }

    public void OnMonsterDeath(MonsterView monster)
    {
        // TODO: 可能的游戏逻辑？
        Debug.Log("怪物死亡: " + monster.monsterName);
        monsters.Remove(monster);
        Destroy(monster.gameObject);
    }
}