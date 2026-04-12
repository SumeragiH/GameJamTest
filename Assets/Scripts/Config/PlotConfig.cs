using System.Collections.Generic;
using UnityEngine;
class PlotConfig: SingletonBaseWithMono<PlotConfig>
{
    // 网格高亮颜色
    [Header("高亮颜色")]
    [SerializeField] public Color placableColor;
    [SerializeField] public Color unplacableColor;
}