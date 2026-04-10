using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlotRow
{
    [SerializeField] public List<PlotTypeEnum> row = new List<PlotTypeEnum>();
}

/// <summary>
/// 某一个地图的地块配置数据
/// 默认一个地图的地块数据是一个二维数组，并且每一行的列数相同
/// 偶数（从0开始计数）行的地块后凹一格，奇数行的地块前凸一格
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "NewMapPlotConfig", menuName = "Data/Map Plot Config")]
public class MapPlotConfigData : ScriptableObject
{
    [SerializeField] private List<PlotRow> plotsConfig = new List<PlotRow>();
    
    public PlotTypeEnum GetPlot(int x, int y)
    {
        if (y >= plotsConfig.Count || x >= plotsConfig[y].row.Count)
            return PlotTypeEnum.None;
        return plotsConfig[y].row[x];
    }

    public int GetColNum()
    {
        if (plotsConfig.Count == 0)
            return 0;
        return plotsConfig[0].row.Count;
    }

    public int GetRowNum()
    {
        return plotsConfig.Count;
    }
}