using UnityEngine;
using System.Collections.Generic;

public class TestSpecialReward : SpecialRewardView
{
    public override TotalProductionData ImprovementProduct()
    {
        // 奖励1策划点
        return new TotalProductionData(1, new List<ProductionData>());
    }

    public TestSpecialReward(PlotView plotView) : base(plotView)
    {
    }
}