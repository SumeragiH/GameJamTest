using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{
    private GameObject Plot; // 点击得到的地块

    [Header("玩家是否可以点击")]
    public bool isClick = true;

    // 推荐：提前缓存射线层
    private int plotLayer;

    private void Start()
    {
        // 缓存Plot层
        plotLayer = LayerMask.NameToLayer("Plot");

        // 注册事件
        EventCenter.Instance.AddListener<GameObject>("点击地块", ClickPlot);
    }

    public void Update()
    {
        if (!isClick) return;

        // 鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 射线检测
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                // 判断点击的是不是地块层
                if (hit.collider.gameObject.layer == plotLayer)
                {
                    // 触发事件
                    EventCenter.Instance.EventTrigger<GameObject>("点击地块", hit.collider.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 事件接收方法
    /// </summary>
    private void ClickPlot(GameObject plot)
    {
        Plot = plot;
        // 在这里可以添加点击地块后的逻辑，例如显示信息、移动相机等
    }
}