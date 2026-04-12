using System.Collections.Generic;
using UnityEngine;

public class TreasureBoxSystem : SingletonBaseWithMono<TreasureBoxSystem>
{
    [field: SerializeField] public List<TreasureBoxView> treasureBoxPrefabList { get; private set; } = new List<TreasureBoxView>();

    public List<TreasureBoxView> GetAllTreasureBoxes()
    {
        return treasureBoxPrefabList;
    }

    public bool AddTreasureBox(TreasureBoxView treasureBoxPrefab, int x, int y)
    {
        if (treasureBoxPrefab == null)
        {
            Debug.LogError("[TreasureBoxSystem] treasureBoxPrefab 为空，无法放置");
            return false;
        }

        PlotView plotView = PlotManageSystem.Instance.GetPlotView(x, y);
        if (plotView == null)
        {
            Debug.LogError($"[TreasureBoxSystem] 目标地块不存在: ({x}, {y})");
            return false;
        }

        if (plotView.treasureBox != null)
        {
            Debug.LogWarning($"[TreasureBoxSystem] 地块 ({x}, {y}) 已有宝箱");
            return false;
        }

        TreasureBoxView treasureBox = Instantiate(treasureBoxPrefab, plotView.transform);
        plotView.treasureBox = treasureBox;
        treasureBox.transform.parent = plotView.transform;
        treasureBox.transform.localPosition = Vector3.zero;
        treasureBox.transform.localRotation = Quaternion.identity;
        return true;
    }
}
