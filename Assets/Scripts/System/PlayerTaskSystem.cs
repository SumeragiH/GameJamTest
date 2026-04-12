using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class PlayerTaskSystem : SingletonBaseWithMono<PlayerTaskSystem>
{
    public List<PlayerTaskView> tasks = new List<PlayerTaskView>();//玩家任务列表 

    public List<PlayerTaskView> compeletedTasks= new List<PlayerTaskView>();//已完成的任务列表,回合结束之后，会根据已经完成的任务进行一些处理，例如增加产出，更新玩家状态等   
    public void AddTask(PlayerTaskView taskData)
    {
        tasks.Add(taskData);
    }

    private void RemoveTask(PlayerTaskView taskData)
    {
        tasks.Remove(taskData);
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
                CompleteTask(task);
            }
        }
    }
    /// <summary>
    /// 完成任务，将任务状态设置为已完成，并进行后续处理，例如增加产出，更新玩家状态等
    /// </summary>
    /// <param name="taskView"></param>
    public void CompleteTask(PlayerTaskView taskView)
    {
        taskView.taskState = playerTaskStateEnum.Completed;
        //TODO
        //任务完成之后的一些处理，例如增加产出，更新玩家状态等

        RemoveTask(taskView);
    }
    /// <summary>
    /// 分配任务给玩家，首先检查策划点数是否足够，然后根据任务类型确定需要的玩家类型，获取符合条件的玩家列表，最后将任务分配给玩家并更新任务和玩家状态
    /// </summary>
    /// <param name="taskView"></param>
    /// <param name="targetPlot"></param>
    public void AssignTask(PlayerTaskView taskView, PlotView targetPlot)
    {
        int costDesignPoint = taskView.taskData.designPointCost*taskView.taskData.setPlayerCount;//根据任务需要的玩家数量计算总的策划点数消耗 
        int DesignPoint = ResourceManageSystem.Instance.GetDesignPoint();
        if(DesignPoint < costDesignPoint)
        {
            //TODO
            //弹窗，提示策划点数不足
            Debug.LogWarning("Not enough design points to assign this task");
            return;
        }
        ResourceManageSystem.Instance.ChangeDesignPoint(costDesignPoint);//消耗策划点数

        //根据任务类型确定玩家类型
        PlayerTaskTypeEnum taskType =  taskView.taskData.taskType;
        PlayerTypeEnum playerType;
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
        List<PlayerView> players = PlayerSystem.Instance.GetPlayersByType(playerType);//获取符合任务类型的玩家列表

        if (players.Count == 0)
        {
            //TODO
            //弹窗，提示没有符合任务类型的玩家
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
        if(assignablePlayers.Count == 0||assignablePlayers.Count < taskView.taskData.setPlayerCount)
        {
            //TODO
            //弹窗，提示没有足够的玩家可分配任务
            Debug.LogWarning("No assignable players available for this task");
            return;
        }

        //分配任务给玩家
        foreach (PlayerView player in assignablePlayers)
        {
            if (taskView.taskData.currentPlayerCount >= taskView.taskData.setPlayerCount)
            {
                //如果已经达到任务允许的最大玩家数量，则停止分配任务
                return;
            }
            if(player.IsBusy())//如果玩家忙碌
            {
                return;
            }
            else
            {
                player.AssignTask(taskView);//分配任务
                //如果没有达到任务允许的最大玩家数量，则继续分配任务
                taskView.taskData.assignedPlayers.Add(player);
                taskView.taskData.currentPlayerCount++;
                AddTask(taskView);
                continue;
            }
        }

    }

    /// <summary>
    /// 这个方法是删除任务的，供外部调用，删除的同时，把关联的玩家状态也要更新，例如将玩家从任务中移除，设置玩家为不忙碌等
    /// </summary>
    /// <param name="taskData"></param>
    public void DeleteTask(PlayerTaskView taskData)
    {
        tasks.Remove(taskData);
        // 更新关联玩家状态
        foreach (var player in taskData.taskData.assignedPlayers)
        {
            player.CancelCurrentTask(); // 取消玩家当前任务
        }
    }

    
}
