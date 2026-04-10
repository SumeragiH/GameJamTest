using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{

    private GameObject lastPlot; // 上一次点击的地块

    [Header("玩家是否可以进行鼠标操作")]
    public bool isMouseAvailable = true;

    // 推荐：提前缓存射线层
    private int plotLayer;

    private void Start()
    {

        // 缓存Plot层
        plotLayer = LayerMask.NameToLayer("Plot");

        // 注册事件
        //EventCenter.Instance.AddListener<GameObject>("左键点击地块", LeftClickPlot);
    }

    public void Update()
    {
        if (!isMouseAvailable) return;


        Ray rayClick = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //射线检测
        if (Physics.Raycast(rayClick, out hit, 1000f))
        {
            // 判断点击的是不是地块层
            if (hit.collider.gameObject.layer == plotLayer)
            {
                GameObject currentPlot = hit.collider.transform.parent.gameObject;

                //如果悬停到新的地块，触发事件
                if (currentPlot != lastPlot)
                {
                    // 离开旧地块
                    if (lastPlot != null)
                    {
                        //Debug.Log("鼠标离开地块：" + lastPlot.name);
                        EventCenter.Instance.EventTrigger<GameObject>("鼠标悬停离开地块", lastPlot);
                    }

                    // 进入新地块
                    //Debug.Log("鼠标进入地块：" + currentPlot.name);
                    lastPlot = currentPlot;
                    EventCenter.Instance.EventTrigger<GameObject>("鼠标悬停进入地块", currentPlot);
                }

                // 点击事件
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
            }
        }
        else
        {
            // 关键：鼠标没有悬停在任何地块上，触发离开事件
            if (lastPlot != null)
            {
                //Debug.Log("鼠标离开所有地块");
                EventCenter.Instance.EventTrigger<GameObject>("鼠标悬停离开地块", lastPlot);
                lastPlot = null;
            }
        }


    }


    private void LeftClickPlot(GameObject plot)
    {
        GameObject Plot = plot;
        Debug.Log("点击了地块：" + Plot.name);
    }
}