using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 管理改良的系统
/// </summary>
class ImprovementSystem : SingletonBaseWithMono<ImprovementSystem>
{
    [field: SerializeField] public List<ImprovementView> improvementPrefabList { get; private set; } = new(); // 改良列表

    public bool AddImprovement(ImprovementView improvementPrefab, int x, int y)
    {
        if (improvementPrefab == null)
        {
            Debug.LogError("[ImprovementSystem] improvementPrefab 为空，无法放置");
            return false;
        }

        PlotView plotView = PlotManageSystem.Instance.GetPlotView(x, y);
        if (plotView != null)
        {
            ImprovementView improvement = Instantiate(improvementPrefab, plotView.transform);
            plotView.improvements.Add(improvement);
            improvement.SetImprovementView(plotView);
            improvement.transform.parent = plotView.transform;
            improvement.transform.localPosition = Vector3.zero;
            improvement.transform.localRotation = Quaternion.identity;
            return true;
        }

        Debug.LogError($"[ImprovementSystem] 目标地块不存在: ({x}, {y})");
        return false;
    }

    /// <summary>
    /// 获取所有改良预制体列表
    /// </summary>
    /// <returns></returns>
    public List<ImprovementView> GetAllImprovements()
    {
        return improvementPrefabList;
    }

    /// <summary>
    /// 根据地块获取可建造的改良预制体列表
    /// </summary>
    /// <param name="plotView"></param>
    /// <returns></returns>
    public List<ImprovementView> GetAvailableImprovements(PlotView plotView)
    {
        List<ImprovementView> availableImprovements = new List<ImprovementView>();
        foreach (var improvement in improvementPrefabList)
        {
            if (improvement.CanBuildOn(plotView))
            {
                availableImprovements.Add(improvement);
            }
        }
        return availableImprovements;
    }
}
