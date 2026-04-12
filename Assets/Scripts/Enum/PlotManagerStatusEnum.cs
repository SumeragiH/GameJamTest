

public enum PlotManagerStatusEnum
{
    Normal, // 正常状态
    PlacingStaff, // 放置单个单元格模式
    PlacingEvent, // 放置事件模式（涉及多个单元格）
    None // 无状态（错误状态）
}