using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 改良，即策划md里面的“建筑“
/// 具体的奖励逻辑在自类中写
/// </summary>
public abstract class ImprovementView : MonoBehaviour
{
    protected PlotView currentPlot; // 所在地块
    public abstract TotalProductionData ImprovementProduct();

    [SerializeField] protected List<PlotTypeEnum> canBuildOn = new(); // 可建造的地块类型列表

    public ImprovementView(PlotView plotView)
    {
        currentPlot = plotView;
    }
}