using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [Header("End Screens Setup")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject canvasControles;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "04_EndScenes")
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
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
                if (winnerPanel != null) winnerPanel.SetActive(true);
            }
        }
    }

    public void CambiarEscena(string nombreEscena)
    {
        if (nombreEscena.Equals("03_Enviroment"))
        {
            StartCoroutine(ShowControlers(nombreEscena));
        }
        else
        {
            SceneManager.LoadScene(nombreEscena);
        }
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

    private IEnumerator ShowControlers(string nombreEscena)
    {
        if (canvasControles != null)
        {
            canvasControles.SetActive(true);
        }
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene(nombreEscena);
    }
}