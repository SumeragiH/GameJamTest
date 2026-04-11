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

    public int x = -1; // 地块横坐标，从0开始计数
    public int y = -1; // 地块纵坐标，从0开始计数
    [field: SerializeField] public PlotTypeEnum plotType {get; private set;} = PlotTypeEnum.None; // 地块类型

    // 改良
    public List<ImprovementView> improvements = new(); // 已经建造的改良

    // 怪物
    public List<MonsterView> monsters = new(); // 地块怪物

    // 特殊奖励
    public List<SpecialRewardView> specialRewards = new(); // 地块特殊奖励

    void Start()
    {
        originalColor = GetComponent<SpriteRenderer>().color; // 存储原始颜色
        EventCenter.Instance.AddListener<GameObject>("鼠标悬停进入地块", HighLightPlot);
        EventCenter.Instance.AddListener<GameObject>("鼠标悬停离开地块", UnHighLightPlot);
        EventCenter.Instance.AddListener<GameObject>("左键点击进入地块", SelectPlot);
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
    //高亮显示地块参数
    [Header("高亮显示地块的参数")]
    public Color greenHighlightColor = new Color(0f, 1f, 0f, 1f);  // 绿色
    public float greenHighlightAlpha = 0.3f;  // 透明度
    public Color blueHighlightColor = new Color(0f, 0f, 1f, 1f);  // 蓝色
    public float blueHighlightAlpha = 0.3f;  // 透明度
    public float ChangeDuration = 0.2f;  // 高亮过渡持续时间
    private Tween colorTween;  // 用于存储当前的颜色过渡
    private Color originalColor;  // 存储原始颜色，方便恢复

    /// <summary>
    /// 高亮显示地块
    /// </summary>
    public void HighLightPlot(GameObject plot)
    {
        if (plot != this.gameObject)
        {
            return;
        }
        colorTween?.Kill();

        SpriteRenderer spriteRenderer = plot.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            return;
        }
        // 计算目标颜色：原色 + 30%绿色
        Color targetColor = Color.Lerp(spriteRenderer.color, greenHighlightColor, greenHighlightAlpha);

        colorTween = spriteRenderer.DOColor(targetColor, ChangeDuration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 取消高亮显示
    /// </summary>
    public void UnHighLightPlot(GameObject plot)
    {
        if (plot != this.gameObject)
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
    #endregion

    public void SelectPlot(GameObject plot)
    {
        if (plot != this.gameObject)
        {
            isSelected = false;
            return;
        }
        isSelected = true;
        colorTween?.Kill();

        SpriteRenderer spriteRenderer = plot.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            return;
        }
        // 计算目标颜色：原色 + 30%蓝色
        Color targetColor = Color.Lerp(spriteRenderer.color, blueHighlightColor, blueHighlightAlpha);

        colorTween = spriteRenderer.DOColor(targetColor, ChangeDuration).SetEase(Ease.OutQuad);
    }

    

    #region 地块被选中时显示信息面板

    #endregion
}