using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Hearts UI Setup")]
    [SerializeField] private List<Image> corazones = new List<Image>();

    [Header("Bottles UI Setup")]
    [SerializeField] private List<Image> botellasOrbes = new List<Image>();

    private GameManager gameManager;
    public bool IsGameOver { set; get; } = false;

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

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void ActualizarCorazones(int vidasActuales)
    {
        for (int i = 0; i < corazones.Count; i++)
        {
            if (corazones[i] != null)
            {
                // Activates or deactivates the heart image depending on current health
                corazones[i].gameObject.SetActive(i < vidasActuales);
            }
        }

        if (vidasActuales <= 0 && gameManager != null)
        {
            IsGameOver = true;
            StartCoroutine(CooldownFinal());
        }
    }

    public void ActivarBotella(int indiceBotella)
    {
        if (indiceBotella >= 0 && indiceBotella < botellasOrbes.Count)
        {
            if (botellasOrbes[indiceBotella] != null)
            {
                botellasOrbes[indiceBotella].gameObject.SetActive(true);
            }
        }
    }

    public void DesactivarBotella(int indiceBotella)
    {
        if (indiceBotella >= 0 && indiceBotella < botellasOrbes.Count)
        {
            if (botellasOrbes[indiceBotella] != null)
            {
                // Highlights or activates the bottle UI element when collected
                botellasOrbes[indiceBotella].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator CooldownFinal()
    {
        yield return new WaitForSeconds(3f);
        gameManager.TriggerFinal();
    }
}