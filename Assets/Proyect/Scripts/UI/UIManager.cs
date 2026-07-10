using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

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
        DesactivarBotella(0);
        DesactivarBotella(1);
        DesactivarBotella(2);

    }

    void Update()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        if (corazones.Exists(heart => heart == null) && escenaActual == "03_Enviroment")
        {
            AddCorazones();
            IsGameOver = false;
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }

        if (botellasOrbes.Exists(bottle => bottle == null) && escenaActual == "03_Enviroment")
        {
            AddBotellas();
            DesactivarBotella(0);
            DesactivarBotella(1);
            DesactivarBotella(2);
        }
    }

    public void AddCorazones()
    {
        corazones.Clear();
        for (int i = 0; i < 5; i++)
        {
            GameObject heart = GameObject.Find("Corazon_" + (i + 1));
            if (heart == null)
            {
                Debug.LogWarning("Heart GameObject not found: Corazon_" + (i + 1));
                continue;
            }
            Image heartImage = heart.GetComponent<Image>();
            corazones.Add(heartImage);
        }
    }

    public void AddBotellas()
    {
        botellasOrbes.Clear();
        for (int i = 0; i < 3; i++)
        {
            GameObject bottle = GameObject.Find("Botella_" + (i + 1));

            if (bottle == null)
            {
                Debug.LogWarning("Bottle GameObject not found: Botella_" + (i + 1));
                continue;
            }
            Image bottleImage = bottle.GetComponent<Image>();
            botellasOrbes.Add(bottleImage);
        }
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
        print("Vidas actuales: " + vidasActuales);
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