using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 50f;
    [SerializeField] private int damage = 10;

    public int Damage {
        set { damage = value; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider otro)
    {
        // 1. Si choca con un enemigo...
        if (otro.CompareTag("Enemigo"))
        {
            // Buscamos el script del enemigo en el objeto que tocamos
            EnemyFollow3 enemigo = otro.GetComponent<EnemyFollow3>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(damage); // Le resta 10 de vida (puedes cambiar este número)
            }

            Destroy(gameObject); // Destruye la bala inmediatamente
        }

        // 2. --- NUEVO: Si choca contra una pared u obstáculo ---
        if (otro.CompareTag("Pared") || otro.CompareTag("Obstaculo"))
        {
            Destroy(gameObject); // La bala del jugador también se destruye al chocar
        }
    }
}
