using UnityEngine;
using System.Collections.Generic;

public class MonsterView : MonoBehaviour
{
    [field: SerializeField] public MonsterTypeEnum MonsterType { get; private set; } = MonsterTypeEnum.None;
    [SerializeField] public int Attack = 0;
    [SerializeField] public int Defense = 0;
    [SerializeField] public int Level = 0;
    [SerializeField] public int Movement = 0; // 移动力, 每轮可以移动的格子数
    [SerializeField] public int Health = 0;

    public void Die()
    {
        EventCenter.Instance.TriggerEvent("怪物死亡", this); // TODO: Event
        Destroy(gameObject);
    }

    public void beAttacked(int damage)
    {
        int actualDamage = Mathf.Max(damage - Defense, 0);
        Health -= actualDamage;
        if (Health <= 0)
        {
            Die();
        }
    }
}