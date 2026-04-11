using UnityEngine;
using System.Collections.Generic;

public class MonsterView : MonoBehaviour
{
    [field: SerializeField] public MonsterTypeEnum monsterType { get; private set; } = MonsterTypeEnum.None;
    [field: SerializeField] public string monsterName { get; private set; } = "怪物";
    [field: SerializeField] public string monsterDescription { get; private set; } = "这是一个怪物";
    [field: SerializeField] public int attack { get; private set; } = 0;
    [field: SerializeField] public int defense { get; private set; } = 0;
    [field: SerializeField] public int level { get; private set; } = 0;
    [field: SerializeField] public int movement { get; private set; } = 0; // 移动力, 每轮可以移动的格子数
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