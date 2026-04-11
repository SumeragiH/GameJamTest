using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateEnum
{
    Idle, // 待机状态，玩家没有进行任何活动
    Working, // 工作状态，玩家正在进行工作活动
    None // 无状态，玩家处于一种特殊状态，可能是游戏开始前或结束后的状态
}
