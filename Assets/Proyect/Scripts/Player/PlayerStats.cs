using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Stats stats;
    private PlayerMovement playerMovement;
    private PlayerActions playerActions;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerActions = GetComponent<PlayerActions>();
        //Velocidad inicial del jugador
        playerMovement.Speed(stats.Speed, 1f);
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
}