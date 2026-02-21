using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 单例模式，方便其他脚本访问（比如 Player 说：我救人了，给老板汇报一下）
    public static GameManager Instance;
    [Header("游戏配置")]
    public float gameTime = 60f;        // 初始倒计时 [cite: 49]
    public int totalSoldiersToSave = 8; // 这一局需要救的人数 [cite: 30]

    [Header("实时状态")]
    public float currentTime;
    public int currentTotalSaved = 0;   // 医院收到的总人数 
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
        if (!isGameActive) return;

        // 1. 处理倒计时 [cite: 48]
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            // 2. 实时同步到 UI
            UIManager.Instance.UpdateHUD(currentTime, player.carryCount, player.maxCarry, currentTotalSaved);
        }
        else
        {
            GameOver(); // 时间到，失败 [cite: 50, 56]
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

    public void AddSavedSoldiers(int amount)
    {
        currentTotalSaved += amount;
        
        // 判定胜利条件 [cite: 51, 54]
        if (currentTotalSaved >= totalSoldiersToSave)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        isGameActive = false;
        Time.timeScale = 0; // 停止游戏
        UIManager.Instance.ShowWin(true); // 显示胜利界面 
    }

    public void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOver(true); // 显示失败界面 
    }

    // 弹窗暂停逻辑 
    public void TogglePause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
}
