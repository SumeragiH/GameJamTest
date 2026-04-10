using UnityEngine;
using System.Collections.Generic;

public class PlotManageSystem : SingletonBaseWithMono<PlotManageSystem>
{
    // 地块列表
    public List<List<PlotView>> plotList {get; private set;} = new List<List<PlotView>>();

    [SerializeField] private MapPlotConfigData mapPlotConfigData; 

    [System.Serializable]
    public class PlotTypeToPrefab
    {
        public PlotTypeEnum plotType;
        public PlotView prefab;
    }

    [SerializeField] private List<PlotTypeToPrefab> plotPrefabs = new List<PlotTypeToPrefab>();
    private Dictionary<PlotTypeEnum, PlotView> plotPrefabsDict = new Dictionary<PlotTypeEnum, PlotView>();

    private int colNum;
    private int rowNum;

    // 用于获取周围地块的offset
    private readonly List<(int, int)> nearbyOffsets = new List<(int, int)>
    {
        (-1, -1), // 左上
        (0, -1),  // 右上
        (1, 0),   // 右
        (0, 1),   // 右下
        (-1, 1),  // 左下
        (-1, 0)   // 左
    };

    void Start()
    {
        colNum = mapPlotConfigData.GetColNum();
        rowNum = mapPlotConfigData.GetRowNum();

        // 将地块类型和预制体的对应关系存入字典，方便后续使用
        foreach (var item in plotPrefabs)
        {
            if (!plotPrefabsDict.ContainsKey(item.plotType))
            {
                plotPrefabsDict.Add(item.plotType, item.prefab);
            }
        }

        // 初始化地块列表
        for (int y = 0; y < rowNum; y++)
        {
            List<PlotView> plotRow = new List<PlotView>();
            for (int x = 0; x < colNum; x++)
            {
                PlotTypeEnum plotType = mapPlotConfigData.GetPlot(x, y);
                if (plotPrefabsDict.ContainsKey(plotType))
                {
                    PlotView plotInstance = Instantiate(plotPrefabsDict[plotType], new Vector3(x, 0, y), Quaternion.identity);
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

    public PlotView getPlotView(int x, int y)
    {
        if (y >= plotList.Count || x >= plotList[y].Count)
            return null;
        return plotList[y][x];
    }

    /// <summary>
    /// 获取一个地块的周围地块 (六个方向)
    /// </summary>
    /// <param name="x">地块的横坐标，从0开始计数</param>
    /// <param name="y">地块的纵坐标，从0开始计数</param>
    /// <returns>返回周围地块的列表，长度一定为6，依次返回左上，右上，右，右下，左下，左的地块。不存在的地块返回null</returns>
    public List<PlotView> getNearbyPlots(int x, int y)
    {
        List<PlotView> nearbyPlots = new List<PlotView>();
        foreach (var offset in nearbyOffsets)
        {
            int newX = x + offset.Item1;
            int newY = y + offset.Item2;
            if (isInMap(newX, newY))
            {
                nearbyPlots.Add(getPlotView(newX, newY));
            }
            else
            {
                nearbyPlots.Add(null);
            }
        }
        return nearbyPlots;
    }

    private bool isInMap(int x, int y)
    {
        return x >= 0 && x < colNum && y >= 0 && y < rowNum;
    }
}