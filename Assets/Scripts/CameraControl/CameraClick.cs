using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{
    private GameObject Plot; // 点击得到的地块

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

        // 射线检测
        if (Physics.Raycast(rayClick, out hit, 1000f))
        {
            // 判断点击的是不是地块层
            if (hit.collider.gameObject.layer == plotLayer)
            {
                // 触发事件
                EventCenter.Instance.EventTrigger<GameObject>("鼠标悬停地块", hit.collider.gameObject);
                //如果点击左键，触发事件，传递点击的地块对象
                if (Input.GetMouseButtonDown(0))
                    EventCenter.Instance.EventTrigger<GameObject>("左键点击地块", hit.collider.gameObject);
                //如果点击右键，触发事件，传递点击的地块对象
                if (Input.GetMouseButtonDown(1))
                    EventCenter.Instance.EventTrigger<GameObject>("右键点击地块", hit.collider.gameObject);
            }
        }

        
    }

    private void LeftClickPlot(GameObject plot)
    {
        Plot = plot;
        Debug.Log("点击了地块：" + Plot.name);
    }
}