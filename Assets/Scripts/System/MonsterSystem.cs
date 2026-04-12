using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物系统：统一驱动刷新点刷新与战斗结算
/// </summary>
public class MonsterSystem : SingletonBaseWithMono<MonsterSystem>
{
    private readonly HashSet<MonsterSpawnerView> spawners = new HashSet<MonsterSpawnerView>();

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
}
