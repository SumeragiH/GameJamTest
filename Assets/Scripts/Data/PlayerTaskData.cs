using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTaskData : MonoBehaviour
{
    [Header("任务基础信息")]
    public int taskID; // 任务ID，唯一标识符
    public string taskName; // 任务名称
    public int maxPlayerCount; // 任务可解取的最大玩家数量
    public int PlayerCount; // 设置的可分配的玩家数量
    public List<PlayerView> assignedPlayers; // 已分配的玩家列表
    public string taskDescription; // 任务描述
    public PlayerTaskTypeEnum taskType; // 任务类型
    public int taskProgress; // 任务进度
    public int taskCostTime; // 任务所需时间，单位为回合数

    [Header("任务奖励")]
    public int rewardDesignPoints; // 任务奖励策划点
    public int rewardMoodPoints; // 任务奖励心情值
    public int rewardExperiencePoints; // 任务奖励经验值
    public List<ProductionData> rewardProductionList; // 任务奖励产出列表

}
