using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD Text")]
    public TextMeshProUGUI timerText; //countdown
    public TextMeshProUGUI carryText; //(0/3)
    public TextMeshProUGUI hospitalText; //saved number

    [Header("other panels")]
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public GameObject restartPopup;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateHUD(float time, int current, int max, int totalSaved)
    {
        timerText.text = "Time: " + Mathf.CeilToInt(time).ToString();
        carryText.text = $"Load: {current}/{max}";
        hospitalText.text = "Saved: " + totalSaved.ToString();
    }

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
        ShowRestartPopup(true);
        GameManager.Instance.TogglePause(true);
    }
}
