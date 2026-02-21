using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD 文本")]
    public TextMeshProUGUI timerText; // 倒计时
    public TextMeshProUGUI carryText; // 载人情况 (0/3) [cite: 16]
    public TextMeshProUGUI hospitalText; // 医院总救人数 [cite: 19]

    [Header("界面层级")]
    public GameObject winPanel; // 胜利界面 [cite: 54]
    public GameObject gameOverPanel; // 失败界面 [cite: 56]
    public GameObject restartPopup; // 重开确认弹窗 [cite: 59]

    void Awake()
    {
        Instance = this;
    }

    // 更新 HUD 数字的方法
    public void UpdateHUD(float time, int current, int max, int totalSaved)
    {
        timerText.text = "Time: " + Mathf.CeilToInt(time).ToString();
        carryText.text = $"Load: {current}/{max}";
        hospitalText.text = "Saved: " + totalSaved.ToString();
    }

    // 控制弹窗的方法
    public void ShowWin(bool show)
    {
        winPanel.SetActive(show);
    }

    public void ShowGameOver(bool show)
    {
        gameOverPanel.SetActive(show);
    }

    public void ShowRestartPopup(bool show)
    {
        restartPopup.SetActive(show);
    }

    public void OpenRestartMenu()
    {
        ShowRestartPopup(true); // 显示弹窗
        GameManager.Instance.TogglePause(true); // 暂停游戏时间
    }
}
