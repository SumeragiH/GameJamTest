using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreasureBoxView : PlacableView
{
    // 本宝箱的产出列表
    [SerializeField] private TotalProductionData productions;

    public TotalProductionData OpenTreasureBox()
    {
        return productions;
    }

    public void DeleteTreasureBox()
    {
        Destroy(gameObject);
    }
}

