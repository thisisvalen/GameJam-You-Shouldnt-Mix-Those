using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private float value;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            switch (powerUpType)
            {
                case PowerUpType.VelocityBoost:
                    playerStats.SetSpeed(value);
                    break;
                case PowerUpType.AttackBoost:
                    playerStats.SetDamage(value);
                    break;
                case PowerUpType.DefenseBoost:
                    playerStats.SetDefense(value);
                    break;
            }
            // Destruir el power-up después de ser recogido
            Destroy(gameObject);
        }
    }
}
