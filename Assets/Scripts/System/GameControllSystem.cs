using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 游戏控制系统，所有系统的上位
/// </summary>
class GameControllSystem: SingletonBaseWithMono<GameControllSystem>
{
    // TODO

    /// <summary>
    /// 点击下一回合时，调用各个系统的NextTurn方法，更新游戏状态
    /// </summary>
    public void NextTurn()
    {
        //玩家任务系统更新任务状态
        PlayerTaskSystem.Instance.TaskNextTurn();
        //玩家系统更新玩家状态
        PlayerSystem.Instance.PlayerNextTurn();
        //TODO
        //其他系统的NextTurn方法，例如资源系统，事件系统等，UI弹窗等

    }
}