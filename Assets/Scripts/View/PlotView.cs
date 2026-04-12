using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlotView : MonoBehaviour
{
    // 产出
    [field: SerializeField]public List<ProductionData> productions {get; private set;} = new(); // 地块产出列表

    // 地块状态
    public bool isDetected {get; private set;} = false; // 是否被探明 

    public bool isSelected {get; private set;} = false; // 是否被选中

    [SerializeField] public bool canPassed {get; private set;} = true; // 是否能通行

    public int x = -1; // 地块横坐标，从0开始计数
    public int y = -1; // 地块纵坐标，从0开始计数
    [field: SerializeField] public PlotTypeEnum plotType {get; private set;} = PlotTypeEnum.None; // 地块类型

    // 改良
    public List<ImprovementView> improvements = new(); // 已经建造的改良

    // 怪物
    public List<MonsterSpawnerView> monsters = new(); // 地块怪物

    // 宝箱
    public TreasureBoxView treasureBox = null; // 地块宝箱

    // 特殊奖励
    public List<SpecialRewardView> specialRewards = new(); // 地块特殊奖励

    void Start()
    {
        originalColor = GetComponent<SpriteRenderer>().color; // 存储原始颜色
        EventCenter.Instance.AddListener<GameObject>("鼠标悬停", HighLightPlot);
        EventCenter.Instance.AddListener<GameObject>("左键点击", SelectPlot);
    }

    /// <summary>
    /// 计算产出的核心方法类
    /// </summary>
    public TotalProductionData PlotProduct()
    {
        TotalProductionData totalProduction = new TotalProductionData(0, new List<ProductionData>());

        // 加上地块的基础产出
        foreach (var production in productions)
        {
            totalProduction += new TotalProductionData(0, new List<ProductionData> { production });
        }

        // 加上改良的额外产出
        foreach (var improvement in improvements)
        {
            totalProduction += improvement.ImprovementProduct();
        }

        // 加上特殊奖励的额外产出
        foreach (var specialReward in specialRewards)
        {
            totalProduction += specialReward.ImprovementProduct();
        }
        return totalProduction;
    }

    public void OnEventApplied(EventView eventView)
    {
        // 处理事件触发逻辑
        eventView.ApplyEvent(this);
    }

    public void OnEventResolved(EventView eventView)
    {
        // 处理事件解决逻辑
        eventView.ResolveEvent(this);
    }

    #region 高亮显示地块
    //高亮显示地块参数
    [Header("高亮显示地块的参数")]
    public Color greenHighlightColor = new Color(0.2f, 1f, 0f, 0.5f);  // 绿色
    public Color blueHighlightColor = new Color(0f, 0.8f, 1f, 1f);  // 蓝色
    public float ChangeDuration = 0.2f;  // 高亮过渡持续时间
    private Tween colorTween;  // 用于存储当前的颜色过渡
    private Color originalColor;  // 存储原始颜色，方便恢复

    /// <summary>
    /// 绿色高亮显示地块（鼠标悬停时）
    /// </summary>
    public void HighLightPlot(GameObject plot)
    {
        // 判断是否为地块
        if (plot == null)
        {
            return;
        }

        // 如果已经被选中，不需要绿色高亮，保持蓝色
        if (isSelected)
        {
            return;
        }

        if (plot != this.gameObject)
        {
            RestoreColor(plot);
            return;
        }

        colorTween?.Kill();

        SpriteRenderer spriteRenderer = plot.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }
        //Debug.Log("高亮显示地块: " + plot.GetComponent<PlotView>().x + ", " + plot.GetComponent<PlotView>().y);
        Color targetColor = greenHighlightColor;
        colorTween = spriteRenderer.DOColor(targetColor, ChangeDuration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 恢复地块颜色到原始状态
    /// </summary>
    public void RestoreColor(GameObject plot)
    {
        // 如果地块被选中了，不恢复颜色
        if (isSelected)
        {
            return;
        }

        if (plot == null || plot != this.gameObject)
        {
            SpriteRenderer SR = GetComponent<SpriteRenderer>();
            if (SR.color == originalColor)
            {
                return;
            }

            colorTween?.Kill();

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                return;
            }

            colorTween = spriteRenderer.DOColor(originalColor, ChangeDuration).SetEase(Ease.OutQuad);
        }
    }

    /// <summary>
    /// 蓝色高亮显示地块，表示选中状态（点击时）
    /// </summary>
    public void SelectPlot(GameObject plot)
    {
        // 判断是否为地块
        if (plot == null || plot.GetComponent<PlotView>() == null)
        {
            return;
        }

        if (plot != this.gameObject)
        {
            isSelected = false;
            RestoreColor(plot);
            return;
        }

        isSelected = true;
        colorTween?.Kill();

        SpriteRenderer spriteRenderer = plot.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }

        Color targetColor = blueHighlightColor;
        colorTween = spriteRenderer.DOColor(targetColor, ChangeDuration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 取消选中地块
    /// </summary>
    public void DeselectPlot()
    {
        isSelected = false;
        RestoreColor(this.gameObject);
    }
    #endregion



    #region 地块被选中时显示信息面板

    #endregion
}