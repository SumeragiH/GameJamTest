using UnityEngine;

/// <summary>
/// 怪物实体（作为预制体组件被MonsterSpawner实例化）
/// </summary>
public class MonsterView : MonoBehaviour
{
    [Header("怪物基础属性")]
    [SerializeField] private MonsterTypeEnum _monsterType = MonsterTypeEnum.None;
    [SerializeField] private string _monsterName = "怪物";
    [SerializeField] private string _monsterDescription = "这是一个怪物";
    [SerializeField] private int _attack = 1;
    [SerializeField] private int _defense = 0;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _movement = 1;
    [SerializeField] private int _maxHealth = 10;

    public MonsterTypeEnum monsterType => _monsterType;
    public string monsterName => _monsterName;
    public string monsterDescription => _monsterDescription;
    public int attack => _attack;
    public int defense => _defense;
    public int level => _level;
    public int movement => _movement;
    public int maxHealth => _maxHealth;
    public int currentHealth { get; private set; }

    private void Awake()
    {
        currentHealth = Mathf.Max(1, _maxHealth);
    }

    public void ResetRuntimeState()
    {
        currentHealth = Mathf.Max(1, _maxHealth);
    }

    public bool BeAttacked(int damage)
    {
        int actualDamage = Mathf.Max(damage - _defense, 0);
        currentHealth -= actualDamage;
        return currentHealth <= 0;
    }
}
