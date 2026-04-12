using UnityEngine;

/// <summary>
/// 改良，即策划md里面的“建筑“
/// 具体的奖励逻辑在自类中写
/// </summary>
public abstract class ImprovementView : PlacableView
{
    protected PlotView currentPlot; // 所在地块
    public abstract TotalProductionData ImprovementProduct();

    public void SetImprovementView(PlotView plotView)
    {
        currentPlot = plotView;
    }
}
