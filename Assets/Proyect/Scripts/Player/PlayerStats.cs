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
        stats.Health -= cantidad;
        Debug.Log("¡Ouch! El jugador recibió " + cantidad + " de daño. Vida restante: " + stats.Health);

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