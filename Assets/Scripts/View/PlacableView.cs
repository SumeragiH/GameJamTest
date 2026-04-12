using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PlacableView : MonoBehaviour
{
    [field: SerializeField]
    public PlacableCategoryEnum category { get; private set; } = PlacableCategoryEnum.Building;

    [field: SerializeField, Min(0)]
    public int designPointCost { get; private set; } = 0;

    [field: SerializeField, FormerlySerializedAs("improvementName"), FormerlySerializedAs("specialRewardName")]
    public string viewName { get; private set; } = "";

    [field: SerializeField, FormerlySerializedAs("description")]
    public string description { get; private set; } = "";

    [SerializeField] protected List<PlotTypeEnum> canBuildOn = new(); // 可建造的地块类型列表

    public bool CanBuildOn(PlotView plotView)
    {
        return plotView != null && canBuildOn.Contains(plotView.plotType);
    }
}
