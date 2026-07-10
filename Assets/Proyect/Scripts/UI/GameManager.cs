using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("End Screens Setup")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winnerPanel;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (winnerPanel != null) winnerPanel.SetActive(false);
        if (UIManager.Instance != null && UIManager.Instance.IsGameOver)
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            if (winnerPanel != null) winnerPanel.SetActive(false);
        }
        else
        {
            if (gameOverPanel != null)gameOverPanel.SetActive(false);
            if (winnerPanel != null) winnerPanel.SetActive(true);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-bind panel references if we just loaded the EndScenes scene
        if (scene.name == "04_EndScenes")
        {
            FindEndScreenPanels();
        }
    }

    public void CambiarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    public void TriggerFinal()
    {
        CambiarEscena("04_EndScenes");
    }

    private void FindEndScreenPanels()
    {
        // Find panels by tag or structure if they are not persistent across scenes
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            if (gameOverPanel != null) gameOverPanel = gameOverPanel.gameObject;
            if (winnerPanel != null) winnerPanel = winnerPanel.gameObject;
        }
    }
}