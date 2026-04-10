using UnityEngine;
using System.Collections.Generic;

public class PlotManageSystem : SingletonBaseWithMono<PlotManageSystem>
{
    // 地块列表
    public List<List<PlotView>> plotList {get; private set;} = new List<List<PlotView>>();

    [SerializeField] private MapPlotConfigData mapPlotConfigData; 

    [SerializeField] private Dictionary<PlotTypeEnum, PlotView> plotPrefab = new Dictionary<PlotTypeEnum, PlotView>();

    private int colNum;
    private int rowNum;

    void Start()
    {
        colNum = mapPlotConfigData.GetColNum();
        rowNum = mapPlotConfigData.GetRowNum();

        // 初始化地块列表
        for (int y = 0; y < rowNum; y++)
        {
            List<PlotView> plotRow = new List<PlotView>();
            for (int x = 0; x < colNum; x++)
            {
                PlotTypeEnum plotType = mapPlotConfigData.GetPlot(x, y);
                if (plotPrefab.ContainsKey(plotType))
                {
                    PlotView plotInstance = Instantiate(plotPrefab[plotType], new Vector3(x, 0, y), Quaternion.identity);
                    plotRow.Add(plotInstance);
                }
                else
                {
                    plotRow.Add(null);
                }
            }
            plotList.Add(plotRow);
        }
    }
}