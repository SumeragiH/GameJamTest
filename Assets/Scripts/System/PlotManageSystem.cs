using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlotManageSystem : SingletonBaseWithMono<PlotManageSystem>
{
    /// <summary>
    /// 地块列表，[y][x]访问，y代表行，x代表列，坐标从0开始计数
    /// </summary>
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
    [SerializeField] private Quaternion plotRotation = Quaternion.Euler(0f,0f,0f); // 地块默认旋转

    [SerializeField] private float plotWidth = 1f; // 地块宽度
    [SerializeField] private float plotHeight = 1f; // 地块高度

    // 是否正在放置状态
    private PlotManagerStatusEnum currentStatus = PlotManagerStatusEnum.Normal;
    private PlacableView placingStaffPrefab = null; // 当前放置的单元格预制体

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
        EventCenter.Instance.AddListener<PlacableView>("地块放置结束", OnStaffPlacingStatusEntered);


        // 测试代码 (temporary)
        Debug.Log("plotList[0][1]: " + plotList[0][1]);
        Debug.Log("0, 1 可用改良： " + ImprovementSystem.Instance.GetAvailableImprovements(plotList[0][1]).Count);
        ImprovementSystem.Instance.AddImprovement(ImprovementSystem.Instance.GetAvailableImprovements(plotList[0][1])[0], 1, 0);

        Debug.Log("测试: 打印地块产出");
        TotalProductionData totalProduction = PlotsProduct();
        Debug.Log(totalProduction.ToString());

    }

    // 测试变量与代码
    // private bool isFirstUpdate = true;
    // [SerializeField] private PlacableView testPlacablePrefab = null;
    // void Update()
    // {
    //     if (isFirstUpdate)
    //     {
    //         OnStaffPlacingStatusEntered(testPlacablePrefab);
    //         isFirstUpdate = false;
    //     }
    // }

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

#region 地块放置相关

    public void OnStaffPlacingStatusEntered(PlacableView staffPrefab)
    {
        currentStatus = PlotManagerStatusEnum.PlacingStaff;
        placingStaffPrefab = staffPrefab;

        // 设置地块高亮
        for (int y = 0; y < rowNum; y++)
        {
            for (int x = 0; x < colNum; x++)
            {
                if (plotList[y][x] != null)
                {
                    if (placingStaffPrefab.CanBuildOn(plotList[y][x]))
                    {
                        plotList[y][x].ShowPlacableHighlight();
                    }
                    else
                    {
                        plotList[y][x].ShowUnplacableHighlight();
                    }
                }
            }
        }


    }

    public void StaffPlacingStatusEnd()
    {
        // 取消地块高亮
        for (int y = 0; y < rowNum; y++)
        {
            for (int x = 0; x < colNum; x++)
            {
                if (plotList[y][x] != null)
                {
                    plotList[y][x].HideHighlight();
                }
            }
        }


        currentStatus = PlotManagerStatusEnum.Normal;
        placingStaffPrefab = null;
        EventCenter.Instance.EventTrigger("地块放置结束");
    }

#endregion


#region 地块基础操作

    /// <summary>
    /// 得到地块的坐标为(x,y)的地块对象，坐标从0开始计数，x表示横坐标，y表示纵坐标。超出地图范围的坐标返回null
    /// </summary>
    /// <param name="x">地块的横坐标，从0开始计数</param>
    /// <param name="y">地块的纵坐标，从0开始计数</param>
    /// <returns>返回指定坐标的地块对象，如果坐标超出地图范围则返回null</returns>
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

    /// <summary>
    /// 检测坐标 (x,y) 是否在地图范围内
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IsInMap(int x, int y)
    {
        return x >= 0 && x < colNum && y >= 0 && y < rowNum;
    }
#endregion

    #region 交换地块位置
    /// <summary>
    /// 移动地块从 (fromX, fromY) 到 (toX, toY)
    /// </summary>
    /// <param name="fromX">源地块X坐标</param>
    /// <param name="fromY">源地块Y坐标</param>
    /// <param name="toX">目标地块X坐标</param>
    /// <param name="toY">目标地块Y坐标</param>
    /// <param name="duration">移动时长</param>
    public void MovePlot(int fromX, int fromY, int toX, int toY, float duration = 0.5f)
    {
        // 边界检查
        if (!IsInMap(fromX, fromY) || !IsInMap(toX, toY))
        {
            Debug.LogError($"地块移动超出边界: ({fromX}, {fromY}) -> ({toX}, {toY})");
            return;
        }

        PlotView sourcePlot = plotList[fromY][fromX];
        PlotView targetPlot = plotList[toY][toX];

        if (sourcePlot == null)
        {
            Debug.LogError($"源地块不存在: ({fromX}, {fromY})");
            return;
        }

        // 交换 plotList 中的引用
        plotList[fromY][fromX] = targetPlot;
        plotList[toY][toX] = sourcePlot;

        // 更新地块的逻辑坐标
        sourcePlot.x = toX;
        sourcePlot.y = toY;
        if (targetPlot != null)
        {
            targetPlot.x = fromX;
            targetPlot.y = fromY;
        }

        // 更新配置数据
        SwapPlotConfig(fromX, fromY, toX, toY);

        // 播放移动动画
        AnimatePlotMovement(sourcePlot, fromX, fromY, toX, toY, duration);
        if (targetPlot != null)
        {
            AnimatePlotMovement(targetPlot, toX, toY, fromX, fromY, duration);
        }

        // 触发事件
        EventCenter.Instance.EventTrigger<PlotView, PlotView>("地块被交换", sourcePlot, targetPlot);
    }

    /// <summary>
    /// 交换配置数据中的地块类型
    /// </summary>
    private void SwapPlotConfig(int x1, int y1, int x2, int y2)
    {
        // 通过反射或创建Setter方法来修改MapPlotConfigData
        // 这里假设已经添加了SetPlot方法到MapPlotConfigData
        PlotTypeEnum type1 = mapPlotConfigData.GetPlot(x1, y1);
        PlotTypeEnum type2 = mapPlotConfigData.GetPlot(x2, y2);

        mapPlotConfigData.SetPlot(x1, y1, type2);
        mapPlotConfigData.SetPlot(x2, y2, type1);
    }

    /// <summary>
    /// 播放地块移动动画
    /// </summary>
    private void AnimatePlotMovement(PlotView plot, int fromX, int fromY, int toX, int toY, float duration)
    {
        // 计算当前位置和目标位置
        Vector3 targetPosition = CalculatePlotPosition(toX, toY);

        // 使用 DOTween 播放移动动画
        plot.transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad);
    }

    /// <summary>
    /// 根据逻辑坐标计算世界坐标
    /// </summary>
    private Vector3 CalculatePlotPosition(int x, int y)
    {
        Vector3 rowStartPos = plotStartPosition.position
            + new Vector3(0, -y * plotHeight, 0)
            + new Vector3((y % 2 == 0) ? 0 : plotWidth / 2, 0, 0)
            + new Vector3(0, 0, -0.01f);

        Vector3 position = new Vector3(x * plotWidth, 0, 0) + rowStartPos;
        return position;
    }
    #endregion
}