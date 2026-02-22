using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("configuration")]
    public float gameTime = 35f; //Initial countdown
    public int totalSoldiersToSave = 8; //number of soldiers

    [Header("Realtime state")]
    public float currentTime;
    public int currentTotalSaved = 0; //total number of soldiers saved
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
        if (isGameActive && Time.timeScale > 0)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                
                UIManager.Instance.UpdateHUD(
                    currentTime,
                    player.carryCount,
                    player.maxCarry,
                    currentTotalSaved
                );
            }
            else
            {
                GameOver();
            }
        }
    }

    public void StartNewGame()
    {
        
        currentTime = gameTime;
        currentTotalSaved = 0;
        isGameActive = true;
        Time.timeScale = 1;

        
        UIManager.Instance.ShowWin(false);
        UIManager.Instance.ShowGameOver(false);
        UIManager.Instance.ShowRestartPopup(false);

        if (spawner != null)
        {
            spawner.SpawnAll();
        }

        
        if (player != null)
        {
            player.ResetPlayer();
        }

        Debug.Log("Game start! Soliders spawned.");
    }

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

        //before checking win condition, update UI to make sure player sees the change
        ForceUpdateUI();

        if (currentTotalSaved >= totalSoldiersToSave)
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        ForceUpdateUI();
        isGameActive = false;
        Time.timeScale = 0;
        UIManager.Instance.ShowWin(true);
    }

    public void GameOver()
    {
        ForceUpdateUI();
        isGameActive = false;
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOver(true);
    }

    //pause logic when show popup
    public void TogglePause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
}
