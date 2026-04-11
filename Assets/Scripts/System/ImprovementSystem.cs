using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 管理改良的系统
/// </summary>
class ImprovementSystem : SingletonBaseWithMono<ImprovementSystem>
{
    [field: SerializeField] public List<ImprovementView> improvementList { get; private set; } = new(); // 改良列表

    public void AddImprovement(ImprovementView improvement, int x, int y)
    {
        PlotView plotView = PlotManageSystem.Instance.GetPlotView(x, y);
        if (plotView != null)
        {
            plotView.improvements.Add(improvement);
            improvement.transform.parent = plotView.transform;
            improvement.transform.localPosition = Vector3.zero;
        }
    }
}