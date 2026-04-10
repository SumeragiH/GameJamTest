using UnityEngine;

public class PlotView : MonoBehaviour
{
    // 产出
    [field: SerializeField]public int grainOutput {get; private set;} = 0; // 粮食
    [field: SerializeField]public int productivityOutput {get; private set;} = 0; // 生产力

    // 地块状态
    public bool isDetected {get; private set;} = false; // 是否被探明 
    [field: SerializeField] public PlotTypeEnum plotType {get; private set;} = PlotTypeEnum.None; // 地块类型

    // 改良
    // public List<Improvement> improvements {get; private set;} = new List<Improvement>(); // 已经建造的改良
}