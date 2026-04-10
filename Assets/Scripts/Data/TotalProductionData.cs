using System.Collections.Generic;
/// <summary>
/// 专门用于总产出(包括策划点，产出列表)
/// </summary>
public class TotalProductionData
{
    public int designPoint = 0; // 策划点
    public List<ProductionData> productionList = new List<ProductionData>(); // 额外产出列表

    public TotalProductionData(int designPoint, List<ProductionData> productionList)
    {
        this.designPoint = designPoint;
        this.productionList = productionList;
    }
}