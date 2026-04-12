using System.ComponentModel;

public enum PlacableCategoryEnum
{
    [Description("建筑")]
    Building,
    [Description("宝箱")]
    TreasureBox,
    [Description("怪物生成点")]
    MonsterSpawner,
    [Description("资源点")]
    ResourcePoint,
    [Description("大事件")]
    MajorEvent,
}
