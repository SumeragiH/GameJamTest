using UnityEngine;

/// <summary>
/// 产出数据类，用于记录产出对应的产出数量
/// </summary>
[System.Serializable]
public class ProductionData
{
    public ProductData product;
    public int amount; // 产出数量

    public ProductionData(ProductData product, int amount)
    {
        this.product = product;
        this.amount = amount;
    }

    public override string ToString()
    {
        return $"[{product?.name ?? "null"}: {amount}]";
    }
}