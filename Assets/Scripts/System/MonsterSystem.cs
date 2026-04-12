using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物系统：统一驱动刷新点刷新与战斗结算
/// </summary>
public class MonsterSystem : SingletonBaseWithMono<MonsterSystem>
{
    private readonly HashSet<MonsterSpawnerView> spawners = new HashSet<MonsterSpawnerView>();
    [field: SerializeField] public List<MonsterView> monsterPrefabs { get; private set; } = new List<MonsterView>();
    [field: SerializeField] public List<MonsterSpawnerView> monsterSpawnerPrefabList { get; private set; } = new List<MonsterSpawnerView>();

    private void Start()
    {
        MonsterSpawnerView[] existingSpawners = FindObjectsOfType<MonsterSpawnerView>();
        foreach (MonsterSpawnerView spawner in existingSpawners)
        {
            RegisterSpawner(spawner);
        }
    }

    private void Update()
    {
        foreach (MonsterSpawnerView spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.TickRefresh(Time.deltaTime);
            }
        }
    }

    public void RegisterSpawner(MonsterSpawnerView spawner)
    {
        if (spawner == null)
        {
            return;
        }

        spawners.Add(spawner);
    }

    public void UnregisterSpawner(MonsterSpawnerView spawner)
    {
        if (spawner == null)
        {
            return;
        }

        spawners.Remove(spawner);
    }

    public bool TryFightOnPlot(PlotView plot, PlayerView fighter, out MonsterSpawnerView spawner, out MonsterView monster)
    {
        spawner = null;
        monster = null;

        if (plot == null || plot.monsterSpawner == null)
        {
            return false;
        }

        MonsterSpawnerView item = plot.monsterSpawner;
        if (item == null || !item.CanFight())
        {
            return false;
        }

        if (!item.TryConsumeMonsterForFight(out monster))
        {
            return false;
        }

        spawner = item;
        EventCenter.Instance.EventTrigger<MonsterSpawnerView, MonsterView>("怪物被击败", spawner, monster);
        Debug.Log($"[MonsterSystem] {fighter?.name ?? "Unknown"} 在 ({plot.x},{plot.y}) 击败了 {monster.monsterName}，剩余 {spawner.monsterCount}/{spawner.maxMonsterCount}");
        return true;
    }

    public List<MonsterSpawnerView> GetAllMonsterSpawners()
    {
        return monsterSpawnerPrefabList;
    }

    public bool AddMonsterSpawner(MonsterSpawnerView monsterSpawnerPrefab, int x, int y)
    {
        if (monsterSpawnerPrefab == null)
        {
            Debug.LogError("[MonsterSystem] monsterSpawnerPrefab 为空，无法放置");
            return false;
        }

        PlotView plotView = PlotManageSystem.Instance.GetPlotView(x, y);
        if (plotView == null)
        {
            Debug.LogError($"[MonsterSystem] 目标地块不存在: ({x}, {y})");
            return false;
        }

        if (plotView.monsterSpawner != null)
        {
            Debug.LogWarning($"[MonsterSystem] 地块 ({x}, {y}) 已有怪物生成点");
            return false;
        }

        MonsterSpawnerView monsterSpawner = Instantiate(monsterSpawnerPrefab, plotView.transform);
        plotView.monsterSpawner = monsterSpawner;
        monsterSpawner.transform.parent = plotView.transform;
        monsterSpawner.transform.localPosition = Vector3.zero;
        monsterSpawner.transform.localRotation = Quaternion.identity;
        return true;
    }
}
