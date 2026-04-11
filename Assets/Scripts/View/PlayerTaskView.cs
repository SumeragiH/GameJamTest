using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTaskView : MonoBehaviour
{
    public PlayerTaskData taskData;
    public PlotView targetPlot;
    public playerTaskStateEnum taskState;

    public void Init(PlayerTaskData data, PlotView plot)
    {
        taskData = data;
        targetPlot = plot;
        RefreshView();
    }

    private void RefreshView()
    {
        // 更新任务视图的显示内容
        // 例如显示任务名称、描述等
    }
}