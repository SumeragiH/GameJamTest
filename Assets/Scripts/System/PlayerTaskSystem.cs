using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTaskSystem : SingletonBaseWithMono<PlayerTaskSystem>
{
    public void AssignTask(PlayerTaskView taskData, PlotView targetPlot)
    {
        PlayerTaskTypeEnum taskType =  taskData.taskData.taskType;
        PlayerTypeEnum playerType;//根据任务类型确定玩家类型
        switch (taskType)
        {
            case PlayerTaskTypeEnum.Fight:
                // assign fight task
                playerType = PlayerTypeEnum.Fighter;
                break;
            case PlayerTaskTypeEnum.Explore:
                // assign explore task
                playerType = PlayerTypeEnum.Explorer;
                break;
            case PlayerTaskTypeEnum.Social:
                // assign social task
                playerType = PlayerTypeEnum.Socializer; 
                break;
            default:
                Debug.LogError("Invalid task type");
                return;
        }
        List<PlayerView> players = playerSystem.Instance.GetPlayersByType(playerType);//获取符合任务类型的玩家列表
        if (players.Count == 0)
        {
            Debug.LogWarning("No players available for this task");
            return;
        }
        foreach(PlayerView player in players)
        {
            if(player.IsBusy())//如果玩家忙碌
            {
                return;
            }
            else
            {
                player.AssignTask(taskData);//分配任务
            }
        }

    }
}
