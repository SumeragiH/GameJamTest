using UnityEngine;
using System.Collections.Generic;

public class TestImprovement : ImprovementView
{
    [SerializeField] private ProductData grainProductData; 

    /// <summary>
    /// 效果：周围1格内的草地每块增加2粮食产出
    /// </summary>
    /// <returns></returns>
    public override TotalProductionData ImprovementProduct()
    {
        List<PlotView> plotViews = PlotManageSystem.Instance.GetNearbyPlots(currentPlot.x, currentPlot.y);
        int grassLandCount = 0;
        foreach (var plotView in plotViews)
        {
            if (plotView != null && plotView.plotType == PlotTypeEnum.Grassland)
            {
                grassLandCount++;
            }
        }
        TotalProductionData data = new TotalProductionData(grassLandCount, new List<ProductionData> { new ProductionData(grainProductData, grassLandCount * 2) });
        return data;
    }
}