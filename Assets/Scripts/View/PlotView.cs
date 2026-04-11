using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlotView : MonoBehaviour
{
    // 产出
    [field: SerializeField]public int grainOutput {get; private set;} = 0; // 粮食
    [field: SerializeField]public int productivityOutput {get; private set;} = 0; // 生产力

    // 地块状态
    public bool isDetected {get; private set;} = false; // 是否被探明 

    public bool isSelected {get; private set;} = false; // 是否被选中
    [field: SerializeField] public PlotTypeEnum plotType {get; private set;} = PlotTypeEnum.None; // 地块类型

    // 改良
    public List<ImprovementView> improvements {get; private set;} = new(); // 已经建造的改良

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