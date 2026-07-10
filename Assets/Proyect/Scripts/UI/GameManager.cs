using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("End Screens Setup")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winnerPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
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

    public void TriggerGameOver()
    {
        CambiarEscena("04_EndScenes");
        // Delay execution or invoke setup on next frame to ensure scene is fully initialized
        StartCoroutine(SetupEndScreen(false));
    }

    public void TriggerWinner()
    {
        CambiarEscena("04_EndScenes");
        StartCoroutine(SetupEndScreen(true));
    }

    private System.Collections.IEnumerator SetupEndScreen(bool isWinner)
    {
        yield return null; // Wait 1 frame for scene objects to load

        if (gameOverPanel != null && winnerPanel != null)
        {
            gameOverPanel.SetActive(!isWinner);
            winnerPanel.SetActive(isWinner);
        }
    }

    private void FindEndScreenPanels()
    {
        // Find panels by tag or structure if they are not persistent across scenes
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform gameOverTransform = canvas.transform.Find("GameOver_Panel");
            Transform winnerTransform = canvas.transform.Find("Winner_Panel");

            if (gameOverTransform != null) gameOverPanel = gameOverTransform.gameObject;
            if (winnerTransform != null) winnerPanel = winnerTransform.gameObject;
        }
    }
}