using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 专门用于总产出(包括策划点，产出列表)
/// 名字有些奇怪（）不知道叫啥好
/// </summary>
[System.Serializable]
public class TotalProductionData
{
    public int designPoint = 0; // 策划点
    public List<ProductionData> productionList = new List<ProductionData>(); // 额外产出列表

    public TotalProductionData(int designPoint, List<ProductionData> productionList)
    {
        this.designPoint = designPoint;
        this.productionList = productionList;
    }

    public static TotalProductionData operator +(TotalProductionData left, TotalProductionData right)
    {
        if (left is null) return right;
        if (right is null) return left;

        int sumDesignPoint = left.designPoint + right.designPoint;

        List<ProductionData> combinedList = new List<ProductionData>();
        Dictionary<string, ProductionData> mergedByName = new Dictionary<string, ProductionData>();

        void MergeList(List<ProductionData> source)
        {
            if (source == null) return;

            foreach (ProductionData data in source)
            {
                if (data == null) continue;

                string productName = data.product?.productName;
                if (string.IsNullOrEmpty(productName))
                {
                    productName = data.product?.name ?? string.Empty;
                }

                if (mergedByName.TryGetValue(productName, out ProductionData existing))
                {
                    existing.amount += data.amount;
                }
                else
                {
                    ProductionData copy = new ProductionData(data.product, data.amount);
                    mergedByName.Add(productName, copy);
                    combinedList.Add(copy);
                }
            }
        }

        MergeList(left.productionList);
        MergeList(right.productionList);

        return new TotalProductionData(sumDesignPoint, combinedList);
    }

    #region 资源操作
    /// <summary>
    /// 得到策划点数量
    /// </summary>
    /// <returns></returns>
    public int GetDesignPoint() => designPoint;
    /// <summary>
    /// 修改策划点数量，delta可以为正（增加）或负（减少），如果减少后数量小于0则置为0
    /// </summary>
    /// <param name="delta">数量变化值</param>
    public void ChangeDesignPoint(int delta)
    {
        designPoint += delta;
        if (designPoint < 0) 
            designPoint = 0;
    }
    /// <summary>
    /// 得到产出列表
    /// </summary>
    /// <returns></returns>
    public List<ProductionData> GetProductionList() => productionList;

    /// <summary>
    /// 修改产出数量，delta可以为正（增加）或负（减少），如果减少后数量小于0则置为0
    /// </summary>
    /// <param name="productName">产出的名称</param>
    /// <param name="delta">数量变化值</param>
    public void ChangeProductionAmount(string productName, int delta)
    {
        if (string.IsNullOrEmpty(productName))
            return;
        ProductionData existing = productionList.FirstOrDefault(p => 
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
    #endregion

    public override string ToString()
    {
        string prodDetails = productionList == null ? "null" :
                            productionList.Count == 0 ? "empty" :
                            string.Join(", ", productionList.Select(p => p?.ToString() ?? "null"));
        return $"[TotalProductionData] DesignPoint: {designPoint}, ProductionList: [{prodDetails}]";
    }
}
