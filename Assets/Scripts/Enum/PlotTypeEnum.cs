using System.ComponentModel;

public enum PlotTypeEnum
{
    [Description("草地")]
    Grassland, // 草地
    [Description("山地")]
    Mountain, // 山地
    [Description("海洋")]
    Sea, // 海洋
    [Description("森林")]
    Forest, // 森林
    [Description("空地块")]
    None, // 暂时无地块
}