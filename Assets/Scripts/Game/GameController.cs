// Scripts/Game/GameController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{
    public BoardGenerator board;
    public Text statusText; // optional

    public void CheckWin(Slot snapped) 
    {
        if (snapped != null && snapped.isTarget)
        {
            if (statusText) statusText.text = "Success!";
        }
        else
        {
            if (statusText) statusText.text = "Place the block on the green cell.";
        }
    }

    public void RestartScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
