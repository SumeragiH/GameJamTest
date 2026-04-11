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
    [SerializeField] private PlayerData playerData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Animator animator;
    [SerializeField] private Canvas taskInfoCanvas;  // 用来显示任务信息

    [Header("移动相关")]
    [SerializeField] private float moveSpeed = 2f;
    private Tween moveTween;

    [Header("任务相关")]
    //private PlayerTask currentTask;
    private float taskProgress = 0f;
    private Coroutine taskExecutionCoroutine;

    // 缓存当前所在的地块
    private PlotView currentPlot;
    private int currentX = -1;
    private int currentY = -1;

    // 状态
    public PlayerStateEnum currentState { get; private set; } = PlayerStateEnum.Idle;

    public PlayerData GetPlayerData() => playerData;
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
        // 不订阅点击事件，因为玩家不需要被选中
    }

    private void OnDestroy()
    {
        if (moveTween != null)
            moveTween.Kill();
        if (taskExecutionCoroutine != null)
            StopCoroutine(taskExecutionCoroutine);
    }

    /// <summary>
    /// 初始化玩家视图
    /// </summary>
    public void InitializePlayer(PlayerData data, int startX, int startY)
    {
        playerData = data;
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

    #region 任务分配和执行

    /// <summary>
    /// 接收任务分配
    /// 这个方法由PlayerManageSystem调用
    /// </summary>
    //public void AssignTask(PlayerTask task)
    //{
    //    if (currentState != PlayerStateEnum.Idle)
    //    {
    //        Debug.LogWarning($"[PlayerView] {playerData.playerName} 不是空闲状态，无法接收新任务");
    //        return;
    //    }

    //    if (task.taskType != playerData.playerType)
    //    {
    //        Debug.LogWarning($"[PlayerView] {playerData.playerName} 的类型 ({playerData.playerType}) 与任务类型 ({task.taskType}) 不匹配");
    //        return;
    //    }

    //    currentTask = task;
    //    task.taskState = PlayerTaskState.Assigned;

    //    Debug.Log($"[PlayerView] {playerData.playerName} 接收到任务: {task}");

    //    // 开始移动到任务地块
    //    MoveToTaskPlot();
    //}

    /// <summary>
    /// 移动到任务所在的地块
    /// </summary>
    //private void MoveToTaskPlot()
    //{
    //    if (currentTask == null || currentTask.targetPlot == null)
    //    {
    //        Debug.LogError("[PlayerView] 任务信息错误，无法移动");
    //        return;
    //    }

    //    // 取消之前的移动
    //    if (moveTween != null)
    //        moveTween.Kill();

    //    PlotView targetPlot = currentTask.targetPlot;
    //    currentState = PlayerStateEnum.Working;  // 设置为工作中（包括移动）

    //    Vector3 targetPosition = targetPlot.transform.position;
    //    float distance = Vector3.Distance(transform.position, targetPosition);
    //    float duration = distance / moveSpeed;

    //    // 播放移动动画
    //    //PlayMoveAnimation();

    //    // 使用DOTween移动
    //    moveTween = transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad)
    //        .OnComplete(() =>
    //        {
    //            // 到达任务地块
    //            currentPlot = targetPlot;
    //            currentX = targetPlot.x;
    //            currentY = targetPlot.y;

    //            PlayIdleAnimation();

    //            Debug.Log($"[PlayerView] {playerData.playerName} 已到达任务地块 ({currentX}, {currentY})");

    //            // 开始执行任务
    //            ExecuteTask();
    //        });

    //    Debug.Log($"[PlayerView] {playerData.playerName} 开始移动到任务地块 ({targetPlot.x}, {targetPlot.y})，耗时 {duration:F2} 秒");
    //}

    /// <summary>
    /// 执行任务
    /// </summary>
    //private void ExecuteTask()
    //{
    //    if (currentTask == null)
    //    {
    //        Debug.LogError("[PlayerView] 没有任务可执行");
    //        return;
    //    }

    //    currentTask.taskState = PlayerTaskState.InProgress;

    //    // 播放工作动画
    //    //PlayWorkAnimation();

    //    string taskName = GetTaskName(currentTask.taskType);
    //    Debug.Log($"[PlayerView] {playerData.playerName} 开始执行任务: {taskName}");

    //    // 启动任务执行协程
    //    if (taskExecutionCoroutine != null)
    //        StopCoroutine(taskExecutionCoroutine);

    //    taskExecutionCoroutine = StartCoroutine(ExecuteTaskCoroutine());
    //}

    /// <summary>
    /// 任务执行协程
    /// </summary>
    //private IEnumerator ExecuteTaskCoroutine()
    //{
    //    float elapsedTime = 0f;
    //    float taskDuration = currentTask.taskDuration;
    //    string taskName = GetTaskName(currentTask.taskType);

    //    while (elapsedTime < taskDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        currentTask.taskProgress = Mathf.Clamp01(elapsedTime / taskDuration);

    //        // 每隔一段时间输出进度信息
    //        if (Mathf.Approximately(elapsedTime % 1f, Time.deltaTime))
    //        {
    //            Debug.Log($"[PlayerView] {playerData.playerName} - {taskName} 进度: {(currentTask.taskProgress * 100):F0}% | " +
    //                $"剩余时间: {(taskDuration - elapsedTime):F1}秒");
    //        }

    //        yield return null;
    //    }

    //    // 任务完成
    //    CompleteTask();
    //}

    /// <summary>
    /// 完成任务
    /// </summary>
    //private void CompleteTask()
    //{
    //    if (currentTask == null)
    //        return;

    //    string taskName = GetTaskName(currentTask.taskType);
    //    Debug.Log($"[PlayerView] {playerData.playerName} 完成了任务: {taskName}");

    //    // 根据任务类型增加经验值和改变心情
    //    int expReward = 0;
    //    int moodReward = 0;

    //    switch (currentTask.taskType)
    //    {
    //        case PlayerTypeEnum.Fighter:
    //            expReward = 10;
    //            moodReward = 15;
    //            Debug.Log($"[PlayerView] 战斗任务完成! 奖励: +{expReward}经验 +{moodReward}心情");
    //            break;
    //        case PlayerTypeEnum.Explorer:
    //            expReward = 12;
    //            moodReward = 20;
    //            Debug.Log($"[PlayerView] 探索任务完成! 奖励: +{expReward}经验 +{moodReward}心情");
    //            break;
    //        case PlayerTypeEnum.Socializer:
    //            expReward = 8;
    //            moodReward = 25;
    //            Debug.Log($"[PlayerView] 社交任务完成! 奖励: +{expReward}经验 +{moodReward}心情");
    //            break;
    //    }

    //    playerData.ExperiencePoints += expReward;
    //    playerData.MoodPoints = Mathf.Min(100, playerData.MoodPoints + moodReward);
    //    playerData.playerState = PlayerStateEnum.Idle;

    //    Debug.Log($"[PlayerView] {playerData.playerName} 当前状态 | 经验值: {playerData.ExperiencePoints} | 心情: {playerData.MoodPoints}");

    //    // 通知系统任务完成
    //    PlayerTaskSystem.Instance.CompleteTask(currentTask.taskID);

    //    // 重置状态
    //    PlayIdleAnimation();
    //    currentState = PlayerStateEnum.Idle;
    //    currentTask = null;
    //    taskProgress = 0f;

    //    EventCenter.Instance.EventTrigger<PlayerView>("玩家任务完成", this);
    //}

    /// <summary>
    /// 获取任务名称
    /// </summary>
    //private string GetTaskName(PlayerTypeEnum taskType)
    //{
    //    return taskType switch
    //    {
    //        PlayerTypeEnum.Fighter => "战斗",
    //        PlayerTypeEnum.Explorer => "探索",
    //        PlayerTypeEnum.Socializer => "社交",
    //        _ => "未知任务"
    //    };
    //}

    #endregion

    #region 动画和视觉反馈

    /// <summary>
    /// 播放空闲动画
    /// </summary>
    //private void PlayIdleAnimation()
    //{
    //    if (animator != null)
    //    {
    //        animator.SetBool("IsMoving", false);
    //        animator.SetBool("IsWorking", false);
    //    }
    //}

    /// <summary>
    /// 播放移动动画
    /// </summary>
    //private void PlayMoveAnimation()
    //{
    //    if (animator != null)
    //    {
    //        animator.SetBool("IsMoving", true);
    //        animator.SetBool("IsWorking", false);
    //    }
    //}

    /// <summary>
    /// 播放工作动画
    /// </summary>
    //private void PlayWorkAnimation()
    //{
    //    if (animator != null)
    //    {
    //        animator.SetBool("IsMoving", false);
    //        animator.SetBool("IsWorking", true);
    //    }
    //}
    #endregion
}