using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 玩家数据类：
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public int playerID; // 玩家ID,唯一标识符
    public string playerName; // 玩家名称，随机生成
    public PlayerTypeEnum playerType; // 玩家类型

    public int playerLevel; // 玩家等级
    public float playerHealth; // 玩家健康值，范围0-100，影响玩家的生存能力
    public float playerAtk; // 玩家攻击力，影响玩家的战斗能力
    public float playerDef; // 玩家防御力，影响玩家的生存能力

    public PlayerStateEnum playerState; // 玩家状态，影响玩家的行为和可执行的活动
    public float MoodPoints; // 心情值，范围0-100，影响玩家的行为和产出效率
    public float ExperiencePoints; // 经验值，玩家通过完成任务和活动获得，达到一定值后升级

    public Sprite playerIcon; // 玩家形象
    public override string ToString()
    {
        return $"{playerName} ({playerType})";
    }
}
