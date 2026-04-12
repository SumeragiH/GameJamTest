using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 策划的资源管理系统，保存策划的数据
/// </summary>
class ResourceManageSystem: SingletonBaseWithMono<ResourceManageSystem>
{
    private TotalProductionData totalProductData = new TotalProductionData(); // 总产出数据

    #region 资源数据操作
    
    /// <summary>
    /// 得到总产出数据（包括策划点和产出列表）
    /// </summary>
    /// <returns></returns>
    public TotalProductionData GetTotalProductData()
    {
        return totalProductData;
    }
    /// <summary>
    /// 得到产出列表
    /// </summary>
    /// <returns></returns>
    public List<ProductionData> GetProductionList()
    {
        return totalProductData.productionList;
    }

    /// <summary>
    /// 得到策划点数量
    /// </summary>
    /// <returns></returns>
    public int GetDesignPoint()
    {
        return totalProductData.designPoint;
    }


    /// <summary>
    /// 修改策划点数量，delta可以为正（增加）或负（减少），如果减少后数量小于0则置为0
    /// </summary>
    /// <param name="delta">数量变化值</param>
    public void ChangeDesignPoint(int delta)
    {
        totalProductData.designPoint += delta;
        if (totalProductData.designPoint < 0)
            totalProductData.designPoint = 0;
    }


    /// <summary>
    /// 修改产出数量，delta可以为正（增加）或负（减少），如果减少后数量小于0则置为0
    /// </summary>
    /// <param name="productName">产出的名称</param>
    /// <param name="delta">数量变化值</param>
    public void ChangeProductionAmount(string productName, int delta)
    {
        if (string.IsNullOrEmpty(productName))
            return;
        ProductionData existing = totalProductData.productionList.FirstOrDefault(p =>
            p != null &&
            p.product != null &&
            ((p.product.productName == productName) || (p.product.name == productName)));
        if (existing != null)
        {
            existing.amount += delta;
            if (existing.amount < 0)
                existing.amount = 0;
        }
    }
    /// <summary>
    /// 添加新的产出数据，newData中的设计点和产出列表会被累加到总产出数据中，如果newData为null则不进行任何操作
    /// </summary>
    /// <param name="newData">新的产出数据</param>
    /// <returns>更新后的总产出数据</returns>
    public TotalProductionData AddProductionData(TotalProductionData newData)
    {
        totalProductData += newData;
        return totalProductData;
    }
    #endregion
}