using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{
    private GameObject currentPlot;
    private GameObject lastPlot;

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

        //射线检测并判断点击的是不是地块层
        if (Physics.Raycast(rayClick, out hit, 1000f) && hit.collider.gameObject.layer == plotLayer)
        {
            if (hit.collider == null) return;
            // 判断点击的是不是地块层
            if (hit.collider.gameObject.layer == plotLayer)
            {
                currentPlot = hit.collider.transform.parent.gameObject;


                //如果悬停到新的地块，触发事件
                if (currentPlot != lastPlot)
                {
                    //Debug.Log("鼠标离开地块：" + lastPlot.name);
                    EventCenter.Instance.EventTrigger<GameObject>("鼠标悬停离开地块", lastPlot);
                }
                // 点击事件
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("左键点击地块：" + currentPlot.name);
                    EventCenter.Instance.EventTrigger<GameObject>("左键点击地块", currentPlot);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    Debug.Log("右键点击地块：" + currentPlot.name);
                    EventCenter.Instance.EventTrigger<GameObject>("右键点击地块", currentPlot);
                }
            }
        }
        else
        {
            // 关键：鼠标没有悬停在任何地块上，触发离开事件
            if (lastPlot != null)
            {
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