using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTaskSystem : SingletonBaseWithMono<PlayerTaskSystem>
{
    public List<PlayerTaskView> tasks = new List<PlayerTaskView>();//玩家任务列表 
    public List<PlayerTaskView> completedTasks = new List<PlayerTaskView>();//已完成的玩家任务列表
    public void AddTask(PlayerTaskView taskData)
    {
        tasks.Add(taskData);
    }

    public void AddCompletedTask(PlayerTaskView taskData)
    {
        //如果任务完成，将其从任务列表中移除，并添加到已完成的任务列表中
        foreach (var item in tasks)
        {
            if (item == taskData)
            {
                RemoveTask(taskData);
            }
        }

        completedTasks.Add(taskData);
    }
    public void RemoveTask(PlayerTaskView taskData)
    {
        tasks.Remove(taskData);
    }

    public void RemoveCompletedTask(PlayerTaskView taskData)
    {
        completedTasks.Remove(taskData);
    }

    public void ClearCompletedTasks()
    {
        completedTasks.Clear();
    }

    /// <summary>
    /// 切换回合时，更新任务状态，减少任务剩余时间，如果任务完成则将其添加到已完成的任务列表中
    /// </summary>
    public void TaskNextTurn()
    {
        foreach (var task in tasks)
        {
            task.taskData.taskCostTime -= 1;
            if (task.taskData.taskCostTime <= 0&&task.taskState==playerTaskStateEnum.Completed)
            {
                AddCompletedTask(task);
            }
        }
    }

    public void AssignTask(PlayerTaskView taskData, PlotView targetPlot)
    {
        //消耗策划点数来分配任务
        //TODO

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
        //得到任务所需要的玩家数量
        List<PlayerView> assignablePlayers = new List<PlayerView>();//可分配任务的玩家列表
        for(int i=0;i<players.Count;i++)
        {
            if(players[i].IsBusy())//如果玩家忙碌
            {
                continue;
            }
            else
            {
                assignablePlayers.Add(players[i]);//将可分配任务的玩家添加到列表中
            }
        }
        if(assignablePlayers.Count == 0||assignablePlayers.Count < taskData.taskData.PlayerCount)
        {
            //弹窗，提示没有足够可分配任务的玩家
            Debug.LogWarning("No assignable players available for this task");
            return;
        }

        //分配任务给玩家
        foreach (PlayerView player in assignablePlayers)
        {
            if(player.IsBusy())//如果玩家忙碌
            {
                return;
            }
            else
            {
                player.AssignTask(taskData);//分配任务
                //如果没有达到任务允许的最大玩家数量，则继续分配任务
                //TODO
                AddTask(taskData);
            }
        }

    }
}
