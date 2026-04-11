using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{
    private GameObject currentPlot;
    private GameObject lastPlot;

    [Header("玩家是否可以进行鼠标操作")]
    public bool isMouseAvailable = true;
    [Header("指针默认指向的空物体")]
    public GameObject DefaultObj;

    // 推荐：提前缓存射线层
    private int plotLayer;

    private void Start()
    {
        // 缓存Plot层
        plotLayer = LayerMask.NameToLayer("Plot");
    }

    public void Update()
    {
        if (!isMouseAvailable)
            return;


        Ray rayClick = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //射线检测，返回所有碰到的物体
        Physics.Raycast(rayClick, out hit, 1000f);
        if (hit.collider!= null)
        {
            currentPlot = hit.collider.transform.parent.gameObject;
        }
        else
        {
            currentPlot = DefaultObj;
        }
        //如果currentPlot为空，就执行离开的事件
        EventCenter.Instance.EventTrigger<GameObject>("鼠标悬停地块", currentPlot);
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("左键点击地块：" + currentPlot.name);
            EventCenter.Instance.EventTrigger<GameObject>("左键点击地块", currentPlot);
        }
        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("右键点击地块：" + currentPlot.name);
            EventCenter.Instance.EventTrigger<GameObject>("右键点击地块", currentPlot);
        }

        if (Input.GetKeyDown(KeyCode.Escape)&&PlotSwapSystem.Instance.isSwapMode)
        {
            EventCenter.Instance.EventTrigger("取消交换模式");
        }

    }


    private void LeftClickPlot(GameObject plot)
    {
        GameObject Plot = plot;
        Debug.Log("点击了地块：" + Plot.name);
    }
}