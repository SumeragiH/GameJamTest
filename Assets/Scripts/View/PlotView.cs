using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PlotView : MonoBehaviour
{
    // 产出
    [field: SerializeField] public List<ProductionData> productions { get; private set; } = new();

    // 地块状态
    public bool isDetected { get; private set; } = false;
    public bool isSelected { get; private set; } = false;
    [SerializeField] public bool canPassed { get; private set; } = true;

    public int x = -1;
    public int y = -1;
    [field: SerializeField] public PlotTypeEnum plotType { get; private set; } = PlotTypeEnum.None;

    // 改良
    public List<ImprovementView> improvements = new();

    // 怪物
    public List<MonsterView> monsters = new();

    // 宝箱
    public TreasureBoxView treasureBox = null;

    // 特殊奖励
    public List<SpecialRewardView> specialRewards = new();

    //Shader实现高亮
    private Material mat;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 获取SpriteRenderer（一次性）
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"PlotView: {gameObject.name} 没有SpriteRenderer组件");
            return;
        }

        // 创建材质实例副本，而不是共享原材质
        mat = spriteRenderer.material;

        // 初始化Shader状态为Normal(0)
        if (mat != null)
        {
            mat.SetFloat("_State", 0);
            // 验证_OutlineWidth是否被正确设置
            Debug.Log($"PlotView({x},{y}) - OutlineWidth: {mat.GetFloat("_OutlineWidth")}");
        }

        // 注册事件监听
        EventCenter.Instance.AddListener<GameObject>("鼠标悬停", HighLightPlot);
        EventCenter.Instance.AddListener<GameObject>("左键点击", SelectPlot);
        EventCenter.Instance.AddListener("取消选中", DeselectPlot); // 如果有这个事件
    }

    void OnDestroy()
    {
        // 移除事件监听，避免内存泄漏
        EventCenter.Instance.RemoveListener<GameObject>("鼠标悬停", HighLightPlot);
        EventCenter.Instance.RemoveListener<GameObject>("左键点击", SelectPlot);
        EventCenter.Instance.RemoveListener("取消选中", DeselectPlot);
    }

    public TotalProductionData PlotProduct()
    {
        TotalProductionData totalProduction = new TotalProductionData(0, new List<ProductionData>());

        foreach (var production in productions)
        {
            totalProduction += new TotalProductionData(0, new List<ProductionData> { production });
        }

        foreach (var improvement in improvements)
        {
            totalProduction += improvement.ImprovementProduct();
        }

        foreach (var specialReward in specialRewards)
        {
            totalProduction += specialReward.ImprovementProduct();
        }
        return totalProduction;
    }

    public void OnEventApplied(EventView eventView)
    {
        eventView.ApplyEvent(this);
    }

    public void OnEventResolved(EventView eventView)
    {
        eventView.ResolveEvent(this);
    }

    #region 高亮显示地块

    /// <summary>
    /// 鼠标悬停时高亮（蓝色边框）
    /// </summary>
    public void HighLightPlot(GameObject plot)
    {
        if(plot == null || plot.GetComponent<PlotView>()==null)
        {
            return;
        }

        // // 如果已经被选中，不需要绿色高亮，保持蓝色
        if (isSelected)
            return;

        if (plot != this.gameObject)
        {
            RestoreColor(plot);
            return;
        }


        if (mat == null)
        {
            Debug.LogError($"Material 为 null：{gameObject.name}");
            return;
        }

        // 设置为Hover状态(1)
        mat.SetFloat("_State", 1);
        Debug.Log($"PlotView({x},{y}) - Hover State Activated");
    }

    /// <summary>
    /// 恢复到正常状态
    /// </summary>
    public void RestoreColor(GameObject plot)
    {
        // 如果已选中，不恢复
        if (isSelected)
            return;

        if (plot != this.gameObject||plot==null)
        {
            if (mat == null)
                return;
            // 设置为Normal状态(0)
            mat.SetFloat("_State", 0);
            Debug.Log($"PlotView({x},{y}) - Normal State Restored");

        }

       
    }

    /// <summary>
    /// 点击选中地块（绿色边框+填充）
    /// </summary>
    public void SelectPlot(GameObject plot)
    {
        // 判断是否为地块
        if (plot == null || plot.GetComponent<PlotView>() == null)
            return;

        // 如果点击的是其他地块，取消其选中状态
        if (plot != this.gameObject)
        {
            isSelected = false;
            RestoreColor(this.gameObject);
            return;
        }

        // 点击自己，标记为选中
        isSelected = true;

        if (mat == null)
        {
            Debug.LogError($"Material 为 null：{gameObject.name}");
            return;
        }

        // 设置为Valid状态(2) - 绿色边框
        mat.SetFloat("_State", 2);
        Debug.Log($"PlotView({x},{y}) - Selected (Valid State)");
    }

    /// <summary>
    /// 取消选中地块
    /// </summary>
    public void DeselectPlot()
    {
        isSelected = false;
        RestoreColor(this.gameObject);
        Debug.Log($"PlotView({x},{y}) - Deselected");
    }

    #endregion
}