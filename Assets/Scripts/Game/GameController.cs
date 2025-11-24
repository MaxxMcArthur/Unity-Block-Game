// Assets/Scripts/Game/GameController.cs
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Core")]
    public BoardGenerator board;
    public DraggableBlock[] blocks;   // assign all 3 block objects in inspector

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject guidePanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject failPanel;

    void Start()
    {
        ShowMainMenu();
    }

    // --------- Screen flow ---------

    public void ShowMainMenu()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopLevelMusic();
            AudioManager.Instance.StopPauseMusic();
        }

        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (guidePanel)    guidePanel.SetActive(false);
        if (gamePanel)     gamePanel.SetActive(false);
        if (pausePanel)    pausePanel.SetActive(false);
        if (winPanel)      winPanel.SetActive(false);
        if (failPanel)     failPanel.SetActive(false);

        ResetLevel();
    }

    public void ShowGuide()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (guidePanel)    guidePanel.SetActive(true);
    }

    public void StartGame()
    {
        ResetLevel();

        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (guidePanel)    guidePanel.SetActive(false);
        if (winPanel)      winPanel.SetActive(false);
        if (failPanel)     failPanel.SetActive(false);

        if (gamePanel)     gamePanel.SetActive(true);
        if (pausePanel)    pausePanel.SetActive(false);

        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelMusic();
        }
    }

    public void PauseGame()
    {
        if (pausePanel) pausePanel.SetActive(true);
        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPauseMusic();
        }
    }

    public void ResumeGame()
    {
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopPauseMusic();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (winPanel)   winPanel.SetActive(false);
        if (failPanel)  failPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);

        if (gamePanel)  gamePanel.SetActive(true);

        ResetLevel();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelMusic();
        }
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        ShowMainMenu();
    }

    void ResetLevel()
    {
        if (board != null)
            board.ClearAllSlots();

        if (blocks != null)
        {
            foreach (var b in blocks)
            {
                if (b != null)
                    b.ResetBlock();
            }
        }
    }

    // --------- Win / fail logic ---------

    // Called by DraggableBlock after a successful placement
    public void OnBlockPlaced()
    {
        // Only care once all blocks have been placed
        if (!AllBlocksPlaced())
            return;

        // Wait a short time so line clear animation & SFX can play
        StartCoroutine(EvaluateEndConditionAfterDelay());
    }

    IEnumerator EvaluateEndConditionAfterDelay()
    {
        // Real-time so it's not affected if we later change timeScale
        yield return new WaitForSecondsRealtime(2f);

        bool hasBlocksOnBoard = (board != null) && board.HasAnyOccupied();

        if (hasBlocksOnBoard)
            ShowFail();
        else
            ShowWin();
    }

    bool AllBlocksPlaced()
    {
        if (blocks == null || blocks.Length == 0)
            return false;

        foreach (var b in blocks)
        {
            if (b == null) continue;
            if (!b.IsPlaced) return false;
        }
        return true;
    }

    void ShowWin()
    {
        Time.timeScale = 0f;
        if (winPanel) winPanel.SetActive(true);
    }

    void ShowFail()
    {
        Time.timeScale = 0f;
        if (failPanel) failPanel.SetActive(true);
    }
}
