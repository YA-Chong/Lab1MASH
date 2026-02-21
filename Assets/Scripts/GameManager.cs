using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 单例模式，方便其他脚本访问（比如 Player 说：我救人了，给老板汇报一下）
    public static GameManager Instance;

    [Header("游戏配置")]
    public float gameTime = 30f; // 初始倒计时 [cite: 49]
    public int totalSoldiersToSave = 8; // 这一局需要救的人数 [cite: 30]

    [Header("实时状态")]
    public float currentTime;
    public int currentTotalSaved = 0; // 医院收到的总人数
    public bool isGameActive = false;
    public WoundedSpawner spawner;
    public PlayerController player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNewGame();
    }

    void Update()
    {
        // 只有在游戏激活 且 没有暂停（时间缩放在运行）时，才处理倒计时
        if (isGameActive && Time.timeScale > 0)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                // 实时同步 UI
                UIManager.Instance.UpdateHUD(
                    currentTime,
                    player.carryCount,
                    player.maxCarry,
                    currentTotalSaved
                );
            }
            else
            {
                GameOver(); // 时间到
            }
        }
    }

    public void StartNewGame()
    {
        // 1. 初始化数据
        currentTime = gameTime;
        currentTotalSaved = 0;
        isGameActive = true;
        Time.timeScale = 1; // 确保时间流逝正常

        // 2. 界面重置
        UIManager.Instance.ShowWin(false);
        UIManager.Instance.ShowGameOver(false);
        UIManager.Instance.ShowRestartPopup(false);

        if (spawner != null)
        {
            spawner.SpawnAll();
        }

        // 2. 重置玩家位置和状态
        if (player != null)
        {
            player.ResetPlayer();
        }

        // 以后这里还会加：重置倒计时、关闭所有 UI 弹窗等
        Debug.Log("游戏开始！士兵已刷新。");
    }

    // 1. 新增一个强制刷新 UI 的私有方法，避免代码重复
    private void ForceUpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHUD(
                currentTime,
                player.carryCount,
                player.maxCarry,
                currentTotalSaved
            );
        }
    }

    public void AddSavedSoldiers(int amount)
    {
        currentTotalSaved += amount;

        // 【关键】在判定胜负前，强制刷新一次 UI，确保玩家看到数字变了
        ForceUpdateUI();

        if (currentTotalSaved >= totalSoldiersToSave)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        ForceUpdateUI(); // 再次确保胜利时刻的数据是准确的
        isGameActive = false;
        Time.timeScale = 0;
        UIManager.Instance.ShowWin(true);
    }

    public void GameOver()
    {
        ForceUpdateUI(); // 确保失败（比如撞树）瞬间的负重和计数显示正确
        isGameActive = false;
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOver(true);
    }

    // 弹窗暂停逻辑
    public void TogglePause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
}
