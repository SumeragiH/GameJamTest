using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIMgr : SingletonBaseWithMono<UIMgr>
{
    //UI字典
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Canvas canvas;//获取UI组件父对象，这样在实例化面板对象时就可以将它设置为这个父对象的子对象，就可以在场景中看到这个面板了

    //获取面板的方法，供外部调用
    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;//获取面板的名字
        if (panelDic.ContainsKey(panelName))//如果字典中已经有这个面板了，就直接返回它
        {
            return panelDic[panelName] as T;
        }
        else//如果字典中没有这个面板，就返回null
        {
            return null;
        }
    }

    private void Start()
    {
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">淡入面板的类型</typeparam>
    /// <param name="show">淡入之后执行的方法，默认值是null</param>
    /// <returns>要显示的面板</returns>
    public T ShowPanel<T>(UnityAction show = null) where T : BasePanel
    {
        string panelName = typeof(T).Name;//获取面板的名字
        if (panelDic.ContainsKey(panelName))//如果字典中已经有这个面板了，就直接显示它
        {
            panelDic[panelName].Show(show);
            return panelDic[panelName] as T;
        }
        else//如果字典中没有这个面板，就创建一个新的面板，并添加到字典中
        {
            GameObject panelPrefab = Resources.Load<GameObject>("UI/" + panelName);//从资源文件夹中加载面板预制体
            if (panelPrefab == null)
            {
                Debug.LogError("没有找到" + panelName + "预制体，请检查资源文件夹中的UI目录下是否有这个预制体");
                return null;
            }
            GameObject panelObj = Instantiate(panelPrefab);//实例化面板对象
            panelObj.transform.SetParent(canvas.transform, false);//将面板对象设置为Canvas对象的子对象，这样就可以在场景中看到这个面板了
            T panel = panelObj.GetComponent<T>();//获取面板对象上的这个面板组件，这个组件是这个面板的类型，里面定义了一些这个面板特有的方法和属性
            panelDic.Add(panelName, panel);//将这个面板添加到字典中，以便下次直接使用
            panel.Show(show);//显示这个面板
            return panel as T;//返回这个面板,方便后续使用
        }
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">淡出面板的类型</typeparam>
    /// <param name="isDestory">是否删除面板，默认值true</param>
    public void HidePanel<T>(bool isDestory = true)
    {
        string panelName = typeof(T).Name;//获取面板的名字
        if (panelDic.ContainsKey(panelName))//如果字典中已经有这个面板了，就直接隐藏它
        {
            panelDic[panelName].Hide(() => {
                if (isDestory)
                {
                    GameObject.Destroy(panelDic[panelName].gameObject);//淡出之后删除
                    panelDic.Remove(panelName);//从字典中移除这个面板
                }
            });
            return;
        }
        else //如果字典中没有这个面板,说明没有显示过这个面板，就直接返回，不需要隐藏了
        {
            return;
        }
    }
}
