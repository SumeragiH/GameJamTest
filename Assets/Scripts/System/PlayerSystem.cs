using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : SingletonBaseWithoutMono<PlayerSystem>
{
    //玩家列表，将同一类型的玩家放在一起，方便管理
    public Dictionary<PlayerTypeEnum,List<PlayerView>> playerDataDictionary;

    public int MaxPlayers;//最大玩家数量（每种类型的玩家数量上限）

    public PlayerSystem()
    {
        playerDataDictionary = new Dictionary<PlayerTypeEnum, List<PlayerView>>();
        // 初始化玩家数据字典，为每种玩家类型创建一个空列表
        foreach (PlayerTypeEnum playerType in System.Enum.GetValues(typeof(PlayerTypeEnum)))
        {
            if (playerType != PlayerTypeEnum.None) // 排除None类型
            {
                playerDataDictionary[playerType] = new List<PlayerView>();
            }
        }
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="playerView"></param>
    public void AddPlayer(PlayerView playerView)
    {
        if(playerView==null)
        { 
            Debug.LogError("尝试添加的玩家为空！");
            return;
        }

        if (playerDataDictionary[playerView.GetPlayerData().playerType].Count >= MaxPlayers)
        {
            //TODO  
            //弹窗提示或者直接将招募界面置灰，无法继续招募
            Debug.LogError("玩家数量已达上限！");
            return;
        }
        if (playerDataDictionary.ContainsKey(playerView.GetPlayerData().playerType))
        {
            playerDataDictionary[playerView.GetPlayerData().playerType].Add(playerView);
        }
        else
        {
            Debug.LogError($"玩家类型 {playerView.GetPlayerData().playerType} 不存在于字典中！");
        }
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    /// <param name="playerView"></param>
    public void RemovePlayer(PlayerView playerView)
    {
        if (playerDataDictionary.ContainsKey(playerView.GetPlayerData().playerType))
        {
            playerDataDictionary[playerView.GetPlayerData().playerType].Remove(playerView);
        }
        else
        {
            Debug.LogError($"玩家类型 {playerView.GetPlayerData().playerType} 不存在于字典中！");
        }
    }

    public void PlayerNextTurn()
    {
        foreach (var playerList in playerDataDictionary.Values)
        {
            foreach (var playerView in playerList)
            {
                //得到每个玩家

                //如果玩家心情值小于等于30，增加坏心情回合数，如果坏心情回合数大于4，玩家死亡
                if (playerView.GetPlayerData().moodPoints <= 30)
                {
                    playerView.GetPlayerData().badMoodTurns += 1;
                    if (playerView.GetPlayerData().badMoodTurns > 4)
                    {
                        playerView.Dead();
                        //删除玩家
                        RemovePlayer(playerView);
                        //TODO
                        //更新UI，弹窗提示玩家死亡
                    }
                }


            }
        }
    }


    public List<PlayerView> GetPlayersByType(PlayerTypeEnum playerType)
    {
        if (playerDataDictionary.ContainsKey(playerType))
        {
            return playerDataDictionary[playerType];
        }
        else
        {
            Debug.LogError($"玩家类型 {playerType} 不存在于字典中！");
            return null;
        }
    }


    public PlayerView GetPlayerByName(string name)
    {
        foreach (var playerList in playerDataDictionary.Values)
        {
            foreach (var playerView in playerList)
            {
                if (playerView.GetPlayerData().playerName == name)
                {
                    return playerView;
                }
            }
        }
        Debug.LogError($"未找到名字为 {name} 的玩家！");
        return null;
    }

    public PlayerView GetPlayerByPosition(int x, int y)
    {
        foreach (var playerList in playerDataDictionary.Values)
        {
            foreach (var playerView in playerList)
            {
                PlotView playerPlot = playerView.GetCurrentPlot();
                if (playerPlot != null && playerPlot.x == x && playerPlot.y == y)
                {
                    return playerView;
                }
            }
        }
        return null;
    }
}
