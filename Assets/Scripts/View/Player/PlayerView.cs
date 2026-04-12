using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家单位视图，表现层
/// 职责：
/// 1. 绑定PlayerData，管理玩家的显示和动画
/// 2. 接收来自PlayerManageSystem的任务分配
/// 3. 自动移动到任务位置并执行任务
/// 4. 处理玩家的状态变化和视觉反馈
/// </summary>
public class PlayerView : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] private PlayerConfigData playerConfigData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Animator animator;
    [SerializeField] private Canvas taskInfoCanvas;  // 用来显示任务信息

    [Header("移动相关")]
    private Tween moveTween;

    [Header("任务相关")]
    private PlayerTaskView currentTask;
    private Coroutine taskExecutionCoroutine;

    // 缓存当前所在的地块
    private PlotView currentPlot;
    private int currentX = -1;
    private int currentY = -1;

    // 状态
    public PlayerStateEnum currentState { get; private set; } = PlayerStateEnum.Idle;

    public PlayerConfigData GetPlayerData() => playerConfigData;
    public PlayerStateEnum GetCurrentState() => currentState;
    public int GetCurrentX() => currentX;
    public int GetCurrentY() => currentY;
    public bool IsBusy() => currentState != PlayerStateEnum.Idle;


    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        //if (animator == null)
        //    animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //监听玩家完成任务事件
        EventCenter.Instance.AddListener<PlayerTaskView>("玩家完成任务", CompleteTask);
    }

    

    /// <summary>
    /// 初始化玩家视图
    /// </summary>
    public void InitializePlayer(PlayerConfigData data, int startX, int startY)
    {
        playerConfigData = data;
        currentX = startX;
        currentY = startY;
        currentPlot = PlotManageSystem.Instance.GetPlotView(startX, startY);

        if (currentPlot != null)
        {
            transform.position = currentPlot.transform.position;
        }

        gameObject.name = $"Player_{data.playerName}_{data.playerType}";
        spriteRenderer.color = Color.white;

        Debug.Log($"[PlayerView] 初始化玩家: {data.playerName} ({data.playerType}) 在位置 ({startX}, {startY})");
    }

    public PlotView GetCurrentPlot() => currentPlot;

    #region 任务分配和执行

    /// <summary>
    /// 接收任务分配
    /// 这个方法由PlayerTaskSystem调用
    /// </summary>
    public void AssignTask(PlayerTaskView task)
    {
        if (currentState != PlayerStateEnum.Idle)
        {
            //TODO
            //弹窗提示玩家当前状态无法接受新任务
            Debug.LogWarning($"[PlayerView] {playerConfigData.playerName} 不是空闲状态，无法接收新任务");
            return;
        }

        currentTask = task;
        task.taskState = playerTaskStateEnum.InProgress;// 设置任务状态为进行中

        // 开始移动到任务地块
        MoveToTaskPlot();
    }

    /// <summary>
    /// 移动到任务所在的地块
    /// </summary>
    private void MoveToTaskPlot()
    {
        if (currentTask == null || currentTask.targetPlot == null)
        {
            //TODO
            //弹窗提示任务信息错误
            Debug.LogError("[PlayerView] 任务信息错误，无法移动");
            //任务信息错误，重置状态
            currentTask = null;
            return;
        }

        // 取消之前的移动
        if (moveTween != null)
            moveTween.Kill();

        PlotView targetPlot = currentTask.targetPlot;
        currentState = PlayerStateEnum.Working;  // 设置为工作中（包括移动）

        Vector3 targetPosition = targetPlot.transform.position;
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / playerConfigData.moveSpeed;

        // 使用DOTween移动
        moveTween = transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                if (currentTask == null)
                {
                    //如果移动过程中任务被取消了，直接返回
                    //TODO
                    //弹窗提示任务信息错误
                    Debug.LogError("[PlayerView] 任务信息错误，无法移动");
                    currentTask = null;
                    return;
                }
                // 到达任务地块
                currentPlot = targetPlot;
                currentX = targetPlot.x;
                currentY = targetPlot.y;

                Debug.Log($"[PlayerView] {playerConfigData.playerName} 已到达任务地块 ({currentX}, {currentY})");

                // 开始执行任务
                ExecuteTask();
            });

        Debug.Log($"[PlayerView] {playerConfigData.playerName} 开始移动到任务地块 ({targetPlot.x}, {targetPlot.y})，耗时 {duration:F2} 秒");
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    private void ExecuteTask()
    {
        if (currentTask == null)
        {
            //TODO
            //弹窗提示没有任务可执行
            Debug.LogError("[PlayerView] 没有任务可执行");
            //没有任务可执行，重置状态
            currentTask = null;
            return;
        }

        currentTask.taskState = playerTaskStateEnum.InProgress;

        // 启动任务执行协程
        if (taskExecutionCoroutine != null)
            StopCoroutine(taskExecutionCoroutine);

        taskExecutionCoroutine = StartCoroutine(ExecuteTaskCoroutine());
    }

    /// <summary>
    /// 任务执行协程
    /// </summary>
    private IEnumerator ExecuteTaskCoroutine()
    {
        //执行任务直接进行数值计算，然后等待回合
        yield return null;
        // 任务完成
        currentTask.taskState = playerTaskStateEnum.Completed;
        // 任务系统和玩家系统都会监听这个事件
        // 触发任务完成事件，传递当前任务信息
        EventCenter.Instance.EventTrigger<PlayerTaskView>("玩家完成任务", currentTask);
    }

    /// <summary>
    /// 完成任务
    /// </summary>
    private void CompleteTask(PlayerTaskView task)
    {
        if (task == null)
            return;

        //string taskName = GetTaskName(task.taskType);
        //Debug.Log($"[PlayerView] {playerData.playerName} 完成了任务: {taskName}");

        //// 根据任务类型增加经验值和改变心情
        //int expReward = 0;
        //int moodReward = 0;

        //switch (currentTask.taskData.taskType)
        //{
        //    case PlayerTaskTypeEnum.Fight: 
        //        if (MonsterSystem.Instance.TryFightOnPlot(currentPlot, this, out MonsterSpawnerView spawner, out MonsterView monster))
        //        {
        //            expReward = 10;
        //            moodReward = 15;
        //            Debug.Log($"[PlayerView] 战斗任务完成，击败 {monster.monsterName}，奖励: +{expReward}经验 +{moodReward}心情");
        //        }
        //        else
        //        {
        //            Debug.LogWarning("[PlayerView] 当前地块没有可战斗怪物，战斗未发生");
        //        }
        //        break;
        //    case PlayerTaskTypeEnum.Explore:
        //        expReward = 12;
        //        moodReward = 20;
        //        Debug.Log($"[PlayerView] 探索任务完成! 奖励: +{expReward}经验 +{moodReward}心情");
        //        break;
        //    case PlayerTaskTypeEnum.Social:
        //        expReward = 8;
        //        moodReward = 25;
        //        Debug.Log($"[PlayerView] 社交任务完成! 奖励: +{expReward}经验 +{moodReward}心情");
        //        break;
        //}

        //playerConfigData.experiencePoints += expReward;
        //playerConfigData.moodPoints = Mathf.Min(100, playerConfigData.moodPoints + moodReward);
        //playerConfigData.playerState = PlayerStateEnum.Idle;

        //Debug.Log($"[PlayerView] {playerConfigData.playerName} 当前状态 | 经验值: {playerConfigData.experiencePoints} | 心情: {playerConfigData.moodPoints}");
        // 通知系统任务完成
        //PlayerTaskSystem.Instance.CompleteTask(currentTask.taskID);

        // 重置状态
        //PlayIdleAnimation();

        currentState = PlayerStateEnum.Idle;
        currentTask = null;

    }

    /// <summary>
    /// 删除任务    
    /// </summary>
    public void CancelCurrentTask()
    {
        if (currentTask == null)
            return;
        // 取消移动
        if (moveTween != null)
            moveTween.Kill();
        // 停止任务执行协程
        if (taskExecutionCoroutine != null)
            StopCoroutine(taskExecutionCoroutine);
        // 重置任务状态
        currentTask.taskState = playerTaskStateEnum.NotStarted;
        currentTask = null;
        // 重置玩家状态
        currentState = PlayerStateEnum.Idle;
        //TODO  
        //弹窗提示任务已取消
        Debug.Log($"[PlayerView] {playerConfigData.playerName} 取消了当前任务");
    }
    #endregion

    //玩家死亡相关
    #region
    private void OnDestroy()
    {
        if (moveTween != null)
            moveTween.Kill();
        if (taskExecutionCoroutine != null)
            StopCoroutine(taskExecutionCoroutine);
    }
    public void Dead()
    {
        // 销毁玩家对象
        Destroy(gameObject);
    }
    #endregion
}
