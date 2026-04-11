using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSystem : SingletonBaseWithoutMono<playerSystem>
{
    //玩家列表，将同一类型的玩家放在一起，方便管理
    public Dictionary<PlayerTypeEnum,List<PlayerData>> playerDataDictionary;

    public playerSystem()
    {
        playerDataDictionary = new Dictionary<PlayerTypeEnum, List<PlayerData>>();
        // 初始化玩家数据字典，为每种玩家类型创建一个空列表
        foreach (PlayerTypeEnum playerType in System.Enum.GetValues(typeof(PlayerTypeEnum)))
        {
            if (playerType != PlayerTypeEnum.None) // 排除None类型
            {
                playerDataDictionary[playerType] = new List<PlayerData>();
            }
        }
    }

    public void AddPlayer(PlayerData playerData)
    {
        if (playerDataDictionary.ContainsKey(playerData.playerType))
        {
            playerDataDictionary[playerData.playerType].Add(playerData);
        }
        else
        {
            Debug.LogError($"玩家类型 {playerData.playerType} 不存在于字典中！");
        }
    }

    public List<PlayerData> GetPlayersByType(PlayerTypeEnum playerType)
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


    public PlayerData GetPlayerByName(string name)
    {
        foreach (var playerList in playerDataDictionary.Values)
        {
            foreach (var playerData in playerList)
            {
                if (playerData.playerName == name)
                {
                    return playerData;
                }
            }
        }
        Debug.LogError($"未找到名字为 {name} 的玩家！");
        return null;
    }

    public PlayerData GetPlayerByPosition(int x, int y)
    {
        foreach (var playerList in playerDataDictionary.Values)
        {
            foreach (var playerData in playerList)
            {
                if (playerData.currentX == x && playerData.currentY == y)
                {
                    return playerData;
                }
            }
        }
        Debug.LogError($"未找到位置为 ({x}, {y}) 的玩家！");
        return null;
    }
}
