using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;   // assign PausePanel in Inspector

    bool isPaused = false;

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // Safety: if this component is disabled, make sure timeScale goes back to normal
    void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
}
