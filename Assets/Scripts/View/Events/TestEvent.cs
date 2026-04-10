using System.Collections;
using UnityEngine;

class TestEvent : EventView
{
    public override void ApplyEvent(PlotView plot)
    {
        Debug.Log("Applying Test Event to plot: " + plot.name);
        // 在这里添加事件应用的逻辑，例如改变地块状态、生成怪物等
    }

    public override void ResolveEvent(PlotView plot)
    {
        Debug.Log("Resolving Test Event on plot: " + plot.name);
        // 在这里添加事件解决的逻辑，例如清除怪物、恢复地块状态等
    }
}