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
    [SerializeField] private Transform plotStartPosition = null; // 地块排布起始位置
    [SerializeField] private Quaternion plotRotation = Quaternion.identity; // 地块默认旋转

    [SerializeField] private float plotWidth = 1f; // 地块宽度
    [SerializeField] private float plotHeight = 1f; // 地块高度

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

        // 排布地块
        for (int y = 0; y < rowNum; y++)
        {
            Vector3 rowStartPos = plotStartPosition.position + new Vector3(0, -y * plotHeight, 0) + new Vector3((y % 2 == 0) ? 0 : plotWidth / 2, 0, 0) + new Vector3(0, 0, -0.01f);
            for (int x = 0; x < colNum; x++)
            {
                if (plotList[y][x] != null)
                {
                    Vector3 position = new Vector3(x * plotWidth, 0, 0) + rowStartPos;
                    plotList[y][x].transform.position = position;
                    plotList[y][x].transform.rotation = plotRotation;
                    plotList[y][x].x = x;
                    plotList[y][x].y = y;
                }
            }
        }

        // 订阅事件
        EventCenter.Instance.AddListener<EventView>("事件生效", OnEventApplied);
        EventCenter.Instance.AddListener<EventView>("事件结束", OnEventResolved);


        // 测试代码 (temporary)
        plotList[2][2].specialRewards.Add(new TestSpecialReward());
        // plotList[2][2].improvements.Add(new TestImprovement(plotList[2][2]));

        Debug.Log("测试: 打印地块产出");
        TotalProductionData totalProduction = PlotsProduct();
        Debug.Log(totalProduction.ToString());
    }

    public TotalProductionData PlotsProduct()
    {
        TotalProductionData totalProduction = new TotalProductionData(0, new List<ProductionData>());

        for (int y = 0; y < rowNum; y++)
        {
            for (int x = 0; x < colNum; x++)
            {
                if (plotList[y][x] != null)
                {
                    totalProduction += plotList[y][x].PlotProduct();
                }
            }
        }
        return totalProduction;
    }

    /// <summary>
    /// 应用特殊事件效果
    /// </summary>
    /// <param name="eventView"></param>
    private void OnEventApplied(EventView eventView)
    {
        for (int y = 0; y < rowNum; y++)
        {
            for (int x = 0; x < colNum; x++)
            {
                if (plotList[y][x] != null)
                {
                    plotList[y][x].OnEventApplied(eventView);
                }
            }
        }
    }

    /// <summary>
    /// 移除特殊事件效果
    /// </summary>
    /// <param name="eventView"></param>
    private void OnEventResolved(EventView eventView)
    {
        for (int y = 0; y < rowNum; y++)
        {
            for (int x = 0; x < colNum; x++)
            {
                if (plotList[y][x] != null)
                {
                    plotList[y][x].OnEventResolved(eventView);
                }
            }
        }
    }

    public PlotView GetPlotView(int x, int y)
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
    public List<PlotView> GetNearbyPlots(int x, int y)
    {
        List<PlotView> nearbyPlots = new List<PlotView>();
        foreach (var offset in nearbyOffsets)
        {
            int newX = x + offset.Item1;
            int newY = y + offset.Item2;
            if (IsInMap(newX, newY))
            {
                nearbyPlots.Add(GetPlotView(newX, newY));
            }
            else
            {
                nearbyPlots.Add(null);
            }
        }
        return nearbyPlots;
    }

    private bool IsInMap(int x, int y)
    {
        return x >= 0 && x < colNum && y >= 0 && y < rowNum;
    }
}