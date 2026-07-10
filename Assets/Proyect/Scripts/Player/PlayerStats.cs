using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Stats stats;
    [SerializeField] private Renderer miRenderer;
    [SerializeField] private Material colorRojo;
    public bool GameOver {set; get;} = false;
    private PlayerMovement playerMovement;
    private PlayerActions playerActions;
    private Material colorOriginal;
    private bool estaParpadeando = false;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerActions = GetComponent<PlayerActions>();
        // Initialize player stats
        playerMovement.Speed(stats.Speed, 1f);
        playerActions.ProjectilePrefab().Damage = (int)stats.Damage;
        if (miRenderer != null)
        {
            colorOriginal = miRenderer.material;
        }
    }
    public float Health => stats.Health;
    public float Damage => stats.Damage;
    public float Defense => stats.Defense;
    public float Speed => stats.Speed;
    public void SetSpeed(float value)
    {
        playerMovement.Speed(value, value/3f);
        stats.Speed = value;
    }
    public void SetDamage(float value)
    {
        playerActions.ProjectilePrefab().Damage = (int)value;
        stats.Damage = value;
    }
    public void SetDefense(float value)
    {
        stats.Defense = value;
    }
    public void SetHealth(float value)
    {
        stats.Health = value;
    }
    public void RecibirDaño(int cantidad)
    {
        // 1. Calculamos el daño real restando la defensa a la cantidad de daño recibido.
        // Usamos Mathf.Max para asegurarnos de que el daño mínimo sea 0 y nunca cure al jugador.
        float dañoReal = Mathf.Max(0f, cantidad - stats.Defense);

        // 2. Restamos el daño real calculado a la vida del jugador
        stats.Health -= dañoReal;

        // 3. Actualizamos los mensajes para mostrar el daño mitigado en la consola
        Debug.Log("¡Ouch! El enemigo infligió " + cantidad + " de daño, pero la Defensa bloqueó " + stats.Defense + ". Daño real recibido: " + dañoReal + ". Vida restante: " + stats.Health);

        if (stats.Health <= 0)
        {
            Debug.Log("¡GAME OVER! El jugador ha sido derrotado.");
            GameOver = true;
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
}