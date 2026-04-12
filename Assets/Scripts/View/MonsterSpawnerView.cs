using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 怪物生成点，可以生成一定数量的怪物，以及一个最大存储怪物数量
/// 有一个刷新时间，刷新过后怪物数量+1
/// </summary>
public class MonsterSpawnerView : MonoBehaviour
{
    [field: SerializeField] public MonsterTypeEnum monsterType { get; private set; } = MonsterTypeEnum.None;
    [field: SerializeField] public string monsterName { get; private set; } = "怪物";
    [field: SerializeField] public string monsterDescription { get; private set; } = "这是一个怪物";
    [field: SerializeField] public int attack { get; private set; } = 0;
    [field: SerializeField] public int defense { get; private set; } = 0;
    [field: SerializeField] public int level { get; private set; } = 0;
    [field: SerializeField] public int movement { get; private set; } = 0; // 移动力, 每轮可以移动的格子数
    [field: SerializeField] public int monsterCount { get; private set; } = 0; // 怪物数量
    [field: SerializeField] public int maxMonsterCount { get; private set; } = 0; // 最大怪物数量
    [field: SerializeField] public int refreshTime { get; private set; } = 0; // 刷新时间，单位为回合
    private int currentRefreshTime = 0; // 当前刷新时间计数
    [SerializeField] public int health = 0;

    public void Die()
    {
        EventCenter.Instance.EventTrigger("怪物死亡", this);
    }

    public void beAttacked(int damage)
    {
        int actualDamage = Mathf.Max(damage - defense, 0);
        health -= actualDamage;
        if (health <= 0)
        {
            Die();
        }
    }
}