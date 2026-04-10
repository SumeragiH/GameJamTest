using UnityEngine;

/// <summary>
/// 产出数据类，用于记录产出的名称、图标
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "NewProductData", menuName = "Data/Product Data")]
public class ProductData : ScriptableObject
{
    public string productName; // 产出名称
    public Sprite productIcon; // 产出图标

    public override string ToString()
    {
        return productName;
    }
}

