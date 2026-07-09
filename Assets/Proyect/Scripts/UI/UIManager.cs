using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Hearts UI Setup")]
    [SerializeField] private List<Image> corazones = new List<Image>();

    [Header("Bottles UI Setup")]
    [SerializeField] private List<Image> botellasOrbes = new List<Image>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        if (vidasActuales <= 0 && GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    public void ActivarBotella(int indiceBotella)
    {
        if (indiceBotella >= 0 && indiceBotella < botellasOrbes.Count)
        {
            if (botellasOrbes[indiceBotella] != null)
            {
                // Highlights or activates the bottle UI element when collected
                botellasOrbes[indiceBotella].gameObject.SetActive(true);
            }
        }
    }
}