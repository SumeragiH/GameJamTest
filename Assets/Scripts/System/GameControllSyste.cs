using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 游戏控制系统，所有系统的上位
/// </summary>
class GameControllSystem: SingletonBaseWithMono<GameControllSystem>
{
    [SerializeField] public int desingnPoint { get; private set; } = 0;

    // TODO
}