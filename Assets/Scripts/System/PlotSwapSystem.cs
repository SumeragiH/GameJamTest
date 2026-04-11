using UnityEngine;

/// <summary>
/// 地块交换系统：
/// 1) 收到“点击移动UI”后进入交换模式
/// 2) 监听两次地块点击
/// 3) 将两个地块的位置交换（transform.position）
///
/// 如果你后续想改成“交换地块数据而非位置”，只要改 SwapPlots() 即可。
/// </summary>
public class PlotSwapSystem :SingletonBaseWithMono<PlotSwapSystem>
{
    [Header("是否交换后自动退出交换模式")]
    [SerializeField] private bool exitAfterOneSwap = true;

    public bool isSwapMode { get; private set; } = false;
    public PlotView firstSelected { get; private set; } = null;

    private void OnEnable()
    {
        EventCenter.Instance.AddListener("点击移动UI", EnterSwapMode);
        EventCenter.Instance.AddListener("取消交换模式", CancelSwapMode);
        EventCenter.Instance.AddListener<GameObject>("左键点击", OnPlotClicked);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveListener("点击移动UI", EnterSwapMode);
        EventCenter.Instance.RemoveListener("取消交换模式", CancelSwapMode);
        EventCenter.Instance.RemoveListener<GameObject>("左键点击", OnPlotClicked);
    }

    private void EnterSwapMode()
    {
        isSwapMode = true;
        firstSelected = null;

        Debug.Log("[PlotSwapSystem] 已进入交换模式，请先后点击两个地块。");
    }

    private void CancelSwapMode()
    {
        isSwapMode = false;
        firstSelected = null;

        Debug.Log("[PlotSwapSystem] 已取消交换模式。");
    }

    private void OnPlotClicked(GameObject Plot)
    {
        PlotView clickedPlot = Plot.GetComponent<PlotView>(); // 确保点击的物体是地块
        if (clickedPlot == null)
        {
            Debug.Log("[PlotSwapSystem] 点击的物体不是地块，已忽略。");
            return;
        }
        if (!isSwapMode) return;

        // 第一次点击
        if (firstSelected == null)
        {
            firstSelected = clickedPlot;
            Debug.Log($"[PlotSwapSystem] 已选择第一个地块: {clickedPlot.name}");
            return;
        }

        // 第二次点击同一个地块：忽略
        if (firstSelected == clickedPlot)
        {
            Debug.Log("[PlotSwapSystem] 两次点击是同一地块，已忽略，请点击另一个地块。");
            return;
        }

        // 执行交换
        PlotManageSystem.Instance.MovePlot(firstSelected.x, firstSelected.y, clickedPlot.x, clickedPlot.y);
        Debug.Log($"[PlotSwapSystem] 交换完成: {firstSelected.name} <-> {clickedPlot.name}");

        // 清理状态
        firstSelected = null;

        if (exitAfterOneSwap)
        {
            isSwapMode = false;
            Debug.Log("[PlotSwapSystem] 已退出交换模式。");
        }
    }

    
}