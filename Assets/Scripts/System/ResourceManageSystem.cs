using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 策划的资源管理系统，保存策划的数据
/// </summary>
class ResourceManageSystem: SingletonBaseWithMono<ResourceManageSystem>
{
    private TotalProductionData totalProductData = new TotalProductionData(); // 总产出数据

    public TotalProductionData GetTotalProductData()
    {
        return totalProductData;
    }

    public TotalProductionData AddProductionData(TotalProductionData newData)
    {
        totalProductData += newData;
        return totalProductData;
    }
}