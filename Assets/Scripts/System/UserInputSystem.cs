using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目前只支持对于地块的输入
/// </summary>
public class UserInputSystem : MonoBehaviour
{
    private GameObject currentGameObject;
    private GameObject lastPlot;

    [Header("玩家是否可以进行鼠标操作")]
    public bool isMouseAvailable = true;
    [Header("指针默认指向的空物体")]
    public GameObject DefaultObj;

    // 指针悬停在同一物体上的时间
    private float hoveringTime = 0;
    // 是否已经在同一GameObject上触发过连续悬停事件
    private bool hasTriggeredContinuousHoverEvent = false;
    private readonly float continuousHoverThreshold = 1f; // 连续悬停事件触发的时间阈值（秒）

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
            if (hit.collider.transform.parent.gameObject != currentGameObject)
            {
                hoveringTime = 0;
                hasTriggeredContinuousHoverEvent = false;
            }
            else
            {
                hoveringTime += Time.deltaTime;
            }
            currentGameObject = hit.collider.transform.parent.gameObject;
        }
        else
        {
            currentGameObject = DefaultObj;
        }
        //如果currentGameObject为空，就执行离开的事件
        EventCenter.Instance.EventTrigger<GameObject>("鼠标悬停", currentGameObject);

        if (!hasTriggeredContinuousHoverEvent && hoveringTime >= continuousHoverThreshold)
        {
            hasTriggeredContinuousHoverEvent = true;
            EventCenter.Instance.EventTrigger<GameObject>("鼠标连续悬停", currentGameObject);
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("左键点击：" + currentGameObject.name);
            EventCenter.Instance.EventTrigger<GameObject>("左键点击", currentGameObject);
        }
        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("右键点击：" + currentGameObject.name);
            EventCenter.Instance.EventTrigger<GameObject>("右键点击", currentGameObject);
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