using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class PlotDesc : BasePanel
{
    [SerializeField] private UIDocument uiDocument;
    private Label plotUpDescText;
    private Label plotDownDescText;

    void Start()
    {
        plotUpDescText = uiDocument.rootVisualElement.Q<Label>("UpText");
        plotDownDescText = uiDocument.rootVisualElement.Q<Label>("DownText");
    }

    public void SetPlotDesc(PlotView plot)
    {
        // 包括地图块类型，改良建筑，怪物，宝箱，特殊奖励
        string upDescText = "";
        // 包括产出和玩家
        string downDescText = "";

        upDescText += $"{plot.plotType.GetDescription()}\n";
        foreach (var improvement in plot.improvements)
        {
            upDescText += $"改良：{improvement.improvementName}\n";
        }
        foreach (var monster in plot.monsters)
        {
            upDescText += $"怪物：{monster.monsterName}\n";
        }
        foreach (var specialReward in plot.specialRewards)
        {
            upDescText += $"特殊奖励：{specialReward.specialRewardName}\n";
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
        
        PlayerView playerView = playerSystem.Instance.GetPlayerByPosition(plot.x, plot.y);
        PlayerConfigData playerData = playerView?.GetPlayerData();
        if (playerData != null)
        {
            downDescText += $"\n{playerData.playerName} 在这个单元格上\n";
        }
        plotUpDescText.text = upDescText;
        plotDownDescText.text = downDescText;
    }
}
