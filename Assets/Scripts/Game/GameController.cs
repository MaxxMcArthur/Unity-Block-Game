// Scripts/Game/GameController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public BoardGenerator board;
    public void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}