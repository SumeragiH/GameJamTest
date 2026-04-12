using System.Collections.Generic;
using UnityEngine;

public class SpecialRewardSystem : SingletonBaseWithMono<SpecialRewardSystem>
{
    [field: SerializeField] public List<SpecialRewardView> specialRewardPrefabList { get; private set; } = new List<SpecialRewardView>();

    public List<SpecialRewardView> GetAllSpecialRewards()
    {
        return specialRewardPrefabList;
    }

    public bool AddSpecialReward(SpecialRewardView specialRewardPrefab, int x, int y)
    {
        if (specialRewardPrefab == null)
        {
            Debug.LogError("[SpecialRewardSystem] specialRewardPrefab 为空，无法放置");
            return false;
        }

        PlotView plotView = PlotManageSystem.Instance.GetPlotView(x, y);
        if (plotView == null)
        {
            Debug.LogError($"[SpecialRewardSystem] 目标地块不存在: ({x}, {y})");
            return false;
        }

        SpecialRewardView specialReward = Instantiate(specialRewardPrefab, plotView.transform);
        plotView.specialRewards.Add(specialReward);
        specialReward.SetSpecialRewardView(plotView);
        specialReward.transform.parent = plotView.transform;
        specialReward.transform.localPosition = Vector3.zero;
        specialReward.transform.localRotation = Quaternion.identity;
        return true;
    }
}
