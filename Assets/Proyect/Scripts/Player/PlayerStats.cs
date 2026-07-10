using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Renderer miRenderer;
    [SerializeField] private Material colorRojo;
    public bool GameOver { set; get; } = false;
    private Input inputPlayer;

    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerActions playerActions;
    private Material colorOriginal;
    private bool estaParpadeando = false;
    private int statsCounter = 0;
    private Coroutine stunCoroutine;
    private bool canResetPowerUp = true;
    private GameManager gameManager;
    private PowerUp[] powerUps;


    void Awake()
    {
        stats.ResetStats();
    }
    private void Start()
    {
        UIManager.Instance.IsGameOver = false;
        GameOver = false;
        playerMovement = GetComponent<PlayerMovement>();
        playerActions = GetComponent<PlayerActions>();
        animator = GetComponent<Animator>();
        animator.SetBool("Death", false); //Por si acaso
        inputPlayer = new Input();
        inputPlayer.Player.Crouch.Enable();
        inputPlayer.Player.Crouch.performed += ResetPowerUp;
        // Initialize player stats

        UIManager.Instance.ActualizarCorazones((int)stats.Health);
        playerMovement.Speed(stats.Speed, 1f);
        playerActions.ProjectilePrefab().Damage = (int)stats.Damage;
        if (miRenderer != null)
        {
            colorOriginal = miRenderer.material;
        }
        powerUps = FindObjectsByType<PowerUp>(FindObjectsSortMode.None);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        int enemiesAlive = GameObject.FindGameObjectsWithTag("Enemigo").Length;
        if (enemiesAlive == 0 && !GameOver)
        {
            GameOver = false;
            UIManager.Instance.IsGameOver = false;
            playerActions.Deactivate();
            playerMovement.Activo = false;
            StartCoroutine(CoolDownGanar());
        }
    }

    public float Health => stats.Health;
    public float Damage => stats.Damage;
    public float Defense => stats.Defense;
    public float Speed => stats.Speed;
    public void SetSpeed(float value)
    {
        playerMovement.Speed(value, value / 3f);
        stats.Speed = value;
        statsCounter++;
        ActivateDebuffs();
        UIManager.Instance.ActivarBotella(2);
    }
    public void SetDamage(float value)
    {
        playerActions.ProjectilePrefab().Damage = (int)value;
        stats.Damage = value;
        statsCounter++;
        UIManager.Instance.ActivarBotella(1);
        ActivateDebuffs();
    }
    public void SetDefense(float value)
    {
        stats.Defense = value;
        statsCounter++;
        UIManager.Instance.ActivarBotella(0);
        ActivateDebuffs();
    }

    public void RecibirDaño(int cantidad)
    {
        // 1. Calculamos el daño real restando la defensa a la cantidad de daño recibido.
        // Usamos Mathf.Max para asegurarnos de que el daño mínimo sea 0 y nunca cure al jugador.
        float dañoReal = Mathf.Max(0f, cantidad - stats.Defense);

        // 2. Restamos el daño real calculado a la vida del jugador
        stats.Health -= dañoReal;
        UIManager.Instance.ActualizarCorazones((int)stats.Health);
        // 3. Actualizamos los mensajes para mostrar el daño mitigado en la consola
        Debug.Log("¡Ouch! El enemigo infligió " + cantidad + " de daño, pero la Defensa bloqueó " + stats.Defense + ". Daño real recibido: " + dañoReal + ". Vida restante: " + stats.Health);

        if (stats.Health <= 0 && !GameOver && SceneManager.GetActiveScene().name == "03_Enviroment")
        {
            Debug.Log("¡GAME OVER! El jugador ha sido derrotado.");
            playerActions.Deactivate();
            playerMovement.Activo = false;
            GameOver = true;
            UIManager.Instance.IsGameOver = true;
            animator.SetBool("Stunned", false);
            animator.SetBool("Death", true);

        }
        else
        {
            StartCoroutine(EfectoGolpe());
        }
    }

    IEnumerator EfectoGolpe()
    {
        if (miRenderer != null && !estaParpadeando)
        {
            estaParpadeando = true;
            miRenderer.material = colorRojo;
            yield return new WaitForSeconds(0.1f);
            miRenderer.material = colorOriginal;
            estaParpadeando = false;
        }
    }

    private void ResetPowerUp(InputAction.CallbackContext ctx)
    {
        if (!canResetPowerUp) return; // Evitar que se reinicie el power-up si no está permitido
        canResetPowerUp = false;
        print("Reset PowerUp");
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stats.ResetStats();
        statsCounter = 0;
        playerMovement.Speed(stats.Speed, 1f);
        playerActions.ProjectilePrefab().Damage = (int)stats.Damage;
        playerMovement.IsCrossing = false;
        StartCoroutine(ColdDownReset());
        for (int i = 0; i < powerUps.Length; i++)
        {
            powerUps[i].ShowPowerUp();
            UIManager.Instance.DesactivarBotella(i);
        }
    }

    private IEnumerator ColdDownReset()
    {
        yield return new WaitForSeconds(2f);
        canResetPowerUp = true;
    }

    public void DontResetPowerUp()
    {
        inputPlayer.Player.Crouch.performed -= ResetPowerUp;
    }

    private void ActivateDebuffs()
    {
        switch (statsCounter)
        {
            case 2:
                if (stunCoroutine == null) stunCoroutine = StartCoroutine(Stunned());
                break;
            case 3:
                playerMovement.IsCrossing = true;
                break;
            default:
                playerMovement.IsCrossing = false;
                if (stunCoroutine != null) StopCoroutine(stunCoroutine);
                break;
        }
    }

    IEnumerator Stunned()
    {
        while (true)
        {
            //Tiempo aleatorio entre 6 y 10 segundos antes de volver a desactivar los controles
            yield return new WaitForSeconds(Random.Range(6f, 10f));
            animator.SetBool("Stunned", true);
            playerMovement.Activo = false;
            playerActions.Deactivate();
            //Tiempo de aturdimiento
            yield return new WaitForSeconds(2f);
            playerMovement.Activo = true;
            animator.SetBool("Stunned", false);
            playerActions.Activate();
        }

    }

    IEnumerator CoolDownGanar()
    {
        yield return new WaitForSeconds(3f);
        gameManager.TriggerFinal();
    }

    void OnDisable()
    {
        inputPlayer.Player.Crouch.performed -= ResetPowerUp;
        inputPlayer.Player.Crouch.Disable();
    }
}