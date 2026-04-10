using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{
    private GameObject Plot;//点击得到的地块

    [Header("玩家是否可以点击")]
    public bool isClick=true;

    private void Start()
    {
        EventCenter.Instance.AddListener<GameObject>("点击地块", ClickPlot);
    }
    public void Update()
    {
        if (!isClick)//如果玩家不能点击，直接返回
            return;
        if (Input.GetMouseButtonDown(0))//如果玩家点击了鼠标左键
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.collider.gameObject.layer==1<<LayerMask.NameToLayer("Plot"))
                {
                    Debug.Log("点击了地块" + hit.collider.gameObject.name);
                    EventCenter.Instance.EventTrigger("点击地块", hit.collider.gameObject);
                }
            }

        }
    }

    private void ClickPlot(GameObject plot)
    {
        Plot = plot;
        Debug.Log("点击了地块" + Plot.name);
    }
}
