using UnityEngine;
using System.Collections.Generic;

public abstract class EventView : PlacableView
{
    public abstract void ApplyEvent(PlotView plot);  
    public abstract void ResolveEvent(PlotView plot);  
}
