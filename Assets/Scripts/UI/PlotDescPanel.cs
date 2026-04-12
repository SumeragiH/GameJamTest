using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class PlotDescPanel : BasePanel
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement plotPanelRoot;
    private Label plotUpDescText;
    private Label plotDownDescText;

    override protected void Awake()
    {
        base.Awake();
        plotPanelRoot = uiDocument.rootVisualElement.Q<VisualElement>("PlotPanel");
        plotUpDescText = uiDocument.rootVisualElement.Q<Label>("UpText");
        plotDownDescText = uiDocument.rootVisualElement.Q<Label>("DownText");
    }

    public void SetPlotDesc(PlotView plot, Vector3 mousePos)
    {
        // 包括地图块类型，改良建筑，怪物，宝箱，特殊奖励
        string upDescText = "";
        // 包括产出和玩家
        string downDescText = "";

        upDescText += $"{plot.plotType.GetDescription()}\n";
        foreach (var improvement in plot.improvements)
        {
            upDescText += $"改良：{improvement.viewName}\n";
        }
        if (plot.monsterSpawner != null && plot.monsterSpawner.monsterCount > 0)
        {
            upDescText += $"怪物：{plot.monsterSpawner.monsterName} x{plot.monsterSpawner.monsterCount}\n";
        }
        foreach (var specialReward in plot.specialRewards)
        {
            upDescText += $"特殊奖励：{specialReward.viewName}\n";
        }
        if (plot.treasureBox != null)
        {
            upDescText += "有 1 个宝箱\n";
        }

        TotalProductionData totalProduction = plot.PlotProduct();
        foreach (var production in totalProduction.productionList)
        {
            downDescText += $"{production.amount} {production.product.productName}\n";
        }
        
        PlayerView playerView = PlayerSystem.Instance.GetPlayerByPosition(plot.x, plot.y);
        PlayerConfigData playerData = playerView?.GetPlayerData();
        if (playerData != null)
        {
            downDescText += $"\n{playerData.playerName} 在这个单元格上\n";
        }

        plotUpDescText.text = upDescText;
        plotDownDescText.text = downDescText;
        UpdatePosition(mousePos);
    }

    private void UpdatePosition(Vector3 mousePos)
    {
        if (plotPanelRoot == null || uiDocument == null || uiDocument.rootVisualElement == null || uiDocument.rootVisualElement.panel == null)
        {
            return;
        }

        const float offsetX = 2f;
        const float offsetY = 2f;

        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(uiDocument.rootVisualElement.panel, mousePos);
        float left = panelPos.x + offsetX;
        float bottom = panelPos.y + offsetY;

        plotPanelRoot.style.left = left;
        plotPanelRoot.style.bottom = bottom;
    }
}
