using UnityEngine;

public class PlotView : MonoBehaviour
{
    // 产出
    [field: SerializeField]public int grainOutput {get; private set;} = 0; // 粮食
    [field: SerializeField]public int productivityOutput {get; private set;} = 0; // 生产力

    // 地块状态
    public bool isDetected {get; private set;} = false; // 是否被探明 

    public bool isSelected {get; private set;} = false; // 是否被选中
    [field: SerializeField] public PlotTypeEnum plotType {get; private set;} = PlotTypeEnum.None; // 地块类型

    // 改良
    public List<ImprovementView> improvements {get; private set;} = new(); // 已经建造的改良

    // 怪物
    public List<MonsterView> monsters = new(); // 地块怪物

    // 特殊奖励
    public List<SpecialRewardView> specialRewards = new(); // 地块特殊奖励

    void Start()
    {
    }

    void OnEventTriggered(EventView eventView)
    {
        // 处理事件触发逻辑
        eventView.ApplyEvent(this);
    }

    void OnEventResolved(EventView eventView)
    {
        // 处理事件解决逻辑
        eventView.ResolveEvent(this);
    }
}