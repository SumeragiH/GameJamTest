using UnityEngine;

/// <summary>
/// 特殊奖励，即策划md里面的“奖励“
/// 具体的奖励逻辑在子类中写
/// </summary>
public abstract class SpecialRewardView : PlacableView
{
    protected PlotView currentPlot; // 所在地块
    public abstract TotalProductionData ImprovementProduct();

    public void SetSpecialRewardView(PlotView plotView)
    {
        currentPlot = plotView;
    }
}
