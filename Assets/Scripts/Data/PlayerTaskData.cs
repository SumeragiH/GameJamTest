using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTaskData : MonoBehaviour
{
    public int taskID; // 任务ID，唯一标识符
    public string taskName; // 任务名称
    public string taskDescription; // 任务描述
    public PlayerTaskTypeEnum taskType; // 任务类型
    public int taskProgress; // 任务进度
    public int taskCostTime; // 任务所需时间，单位为回合数
}
