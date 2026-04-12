using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物刷新点：一个刷新点仅绑定一种怪物预制体
/// </summary>
public class MonsterSpawnerView : PlacableView
{
    [Header("怪物预制体（单一种类）")]
    [SerializeField] private MonsterView _monsterPrefab;

    [Header("刷新参数")]
    [SerializeField] private int _maxMonsterCount = 3;
    [SerializeField] private int _monsterCount = 0;
    [SerializeField] private float _refreshIntervalSeconds = 30f;

    private readonly List<MonsterView> cachedMonsters = new List<MonsterView>();
    private float refreshTimer = 0f;

    public MonsterView monsterPrefab => _monsterPrefab;
    public int maxMonsterCount => _maxMonsterCount;
    public int monsterCount => _monsterCount;
    public float refreshIntervalSeconds => _refreshIntervalSeconds;
    public string monsterName => _monsterPrefab != null ? _monsterPrefab.monsterName : "怪物";

    private void Awake()
    {
        _maxMonsterCount = Mathf.Max(0, _maxMonsterCount);
        _monsterCount = Mathf.Clamp(_monsterCount, 0, _maxMonsterCount);

        cachedMonsters.Clear();
        for (int i = 0; i < _monsterCount; i++)
        {
            SpawnOneMonster();
        }
    }

    private void OnEnable()
    {
        MonsterSystem.Instance.RegisterSpawner(this);
    }

    private void OnDisable()
    {
        MonsterSystem.Instance.UnregisterSpawner(this);
    }

    public void TickRefresh(float deltaTime)
    {
        if (_monsterPrefab == null || _monsterCount >= _maxMonsterCount || _refreshIntervalSeconds <= 0f)
        {
            return;
        }

        refreshTimer += deltaTime;
        while (refreshTimer >= _refreshIntervalSeconds && _monsterCount < _maxMonsterCount)
        {
            refreshTimer -= _refreshIntervalSeconds;
            SpawnOneMonster();
        }
    }

    public bool CanFight()
    {
        return _monsterCount > 0;
    }

    public bool TryConsumeMonsterForFight(out MonsterView defeatedMonster)
    {
        defeatedMonster = null;
        if (!CanFight())
        {
            return false;
        }

        int lastIndex = cachedMonsters.Count - 1;
        if (lastIndex < 0)
        {
            _monsterCount = 0;
            EventCenter.Instance.EventTrigger<MonsterSpawnerView>("怪物刷新点数量变化", this);
            return false;
        }

        defeatedMonster = cachedMonsters[lastIndex];
        cachedMonsters.RemoveAt(lastIndex);
        _monsterCount = Mathf.Max(0, _monsterCount - 1);

        if (defeatedMonster != null)
        {
            Destroy(defeatedMonster.gameObject);
        }

        EventCenter.Instance.EventTrigger<MonsterSpawnerView>("怪物刷新点数量变化", this);
        return true;
    }

    private void SpawnOneMonster()
    {
        if (_monsterPrefab == null)
        {
            Debug.LogError($"[MonsterSpawnerView] {name} 未配置 MonsterView prefab");
            return;
        }

        MonsterView instance = Instantiate(_monsterPrefab, transform.position, Quaternion.identity, transform);
        instance.ResetRuntimeState();
        instance.gameObject.SetActive(false);

        cachedMonsters.Add(instance);
        _monsterCount += 1;
        EventCenter.Instance.EventTrigger<MonsterSpawnerView>("怪物刷新点数量变化", this);
    }
}
